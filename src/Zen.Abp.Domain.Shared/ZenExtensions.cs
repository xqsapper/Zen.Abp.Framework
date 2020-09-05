using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Domain.Shared
{
    public static class ZenExtensions
    {
        public static ApiResult<T> ToApiResult<T>(this T data,
            string message = "",
            ApiResultCode resultCode = ApiResultCode.Success)
        {
            var result = new ApiResult<T>
            {
                Code = (int) resultCode,
                Message = message,
                Data = data
            };
            return result;
        }

        #region Des加密、解密

        //默认密钥向量数组
        private static readonly byte[] RgbIvBytes = {0x06, 0xAB, 0xE4, 0xF2, 0x12, 0x29, 0xDC, 0x11};

        private static readonly byte[] RgbKeyBytes = Encoding.UTF8.GetBytes("Zen@Arch"); //默认密钥,只能8字符

        /// <summary>
        ///     Des加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <returns></returns>
        public static string GetEncryptDes(this string encryptString)
        {
            try
            {
                var inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                var dCsp = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, dCsp.CreateEncryptor(RgbKeyBytes, RgbIvBytes),
                    CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     des解密
        /// </summary>
        /// <param name="decryptString">需要解密的字符串</param>
        /// <returns></returns>
        public static string GetDecryptDes(this string decryptString)
        {
            try
            {
                var inputByteArray = Convert.FromBase64String(decryptString);
                var dCsp = new DESCryptoServiceProvider();
                var mStream = new MemoryStream();
                var cStream = new CryptoStream(mStream, dCsp.CreateDecryptor(RgbKeyBytes, RgbIvBytes),
                    CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return null;
            }
        }

        #endregion Des加密、解密


        #region 其他扩展

        /// <summary>
        ///     将对象转换为Json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        ///     将Json转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        ///     字符串去除空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EnsureTrim(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return string.Empty;
            }

            return str.Trim();
        }

        /// <summary>
        ///     对象中string类型属性去除空格
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void TrimStringProps<T>(this T obj) where T : class
        {
            var props = obj.GetType().GetProperties().Where(m => m.PropertyType == typeof(string));
            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                {
                    continue;
                }

                var newValue = (prop.GetValue(obj)?.ToString()).EnsureTrim();
                var propNewValue = Convert.ChangeType(newValue, prop.PropertyType);
                prop.SetValue(obj, propNewValue, null);
            }
        }

        /// <summary>
        ///     变更实体中的属性值
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="entity"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static TEntity ChangeValue<TEntity, TDto>(this TEntity entity, TDto dto)
            where TEntity : class where TDto : class
        {
            var dtoProps = dto.GetType().GetProperties().Where(m => m.CanWrite).ToArray();
            var entityProps = entity.GetType().GetProperties().Where(m => m.CanWrite).ToArray();
            foreach (var dtoProp in dtoProps)
            {
                var entityProp = entityProps.FirstOrDefault(m => m.CanWrite && m.Name == dtoProp.Name);
                if (entityProp == null)
                {
                    continue;
                }

                var dtoValue = dtoProp.GetValue(dto);
                if (dtoProp.PropertyType == typeof(string))
                {
                    dtoValue = (dtoValue?.ToString() ?? string.Empty).Trim();
                }

                var entityValue = dtoValue == null ? null : ChangeType(dtoValue, entityProp.PropertyType);
                entityProp.SetValue(entity, entityValue, null);
            }

            return entity;
        }

        /// <summary>
        ///     设置对象属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="propNewValue"></param>
        public static void SetProps<T>(this T obj, string propName, object propNewValue) where T : class
        {
            var prop = obj.GetType().GetProperties()
                .First(m => string.Equals(m.Name, propName, StringComparison.OrdinalIgnoreCase));
            if (prop.CanWrite)
            {
                prop.SetValue(obj, propNewValue, null);
            }
        }

        private static object ChangeType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value != null)
                {
                    var nullableConverter = new NullableConverter(conversionType);
                    conversionType = nullableConverter.UnderlyingType;
                }
                else
                {
                    return null;
                }
            }

            return Convert.ChangeType(value, conversionType);
        }

        #endregion
    }
}