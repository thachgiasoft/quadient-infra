using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Core.Settings
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    /// 
    [Serializable]
    public class Setting : ISetting
    {

        /// <summary>
        /// Gets or sets the AppSettingId
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [Required]
        [Display(Name = "Setting Name")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [Required]
        [Display(Name = "Setting Value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [Display(Name = "Encrypt?")]
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// Gets or sets the store for which this setting is valid. 0 is set when the setting is for all stores
        /// </summary>
        //public int StoreId { get; set; }

        public override string ToString()
        {
            return Key;
        }
    }
}
