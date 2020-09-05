using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Volo.Abp.Application.Dtos;

// ReSharper disable once CheckNamespace
namespace Zen.Abp.Application.Contracts
{
    public class ZenPagedAndSortedInDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        ///     跳过多少条
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public override int SkipCount { get; set; }

        /// <summary>
        ///     每页显示多少条数据
        /// </summary>
        [Required]
        [Range(1, 100)]
        public override int MaxResultCount { get; set; }

        /// <summary>
        ///     预留分页条件表达式
        /// </summary>
        [StringLength(500)]
        public override string Sorting { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaxResultCount > 100)
            {
                yield return new ValidationResult($"{nameof(MaxResultCount)}必须在1~100之间",
                    new string[1] {nameof(MaxResultCount)});
            }

            TrimStringProps();
        }

        private void TrimStringProps()
        {
            var props = GetType().GetProperties().Where(m => m.PropertyType == typeof(string));
            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                {
                    continue;
                }

                var newValue = (prop.GetValue(this)?.ToString() ?? string.Empty).Trim();
                var propNewValue = Convert.ChangeType(newValue, prop.PropertyType);
                prop.SetValue(this, propNewValue, null);
            }
        }
    }
}