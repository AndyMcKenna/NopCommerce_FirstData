using System.Collections.Generic;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BitShift.Plugin.Payments.FirstData.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.SandboxURL")]
        public string SandboxURL { get; set; }

        [NopResourceDisplayName("BitShift.Plugin.FirstData.Fields.ProductionURL")]
        public string ProductionURL { get; set; }

        public IList<SelectListItem> Stores { get; set; }

        public bool SavedSuccessfully { get; set; }

        public string SaveMessage { get; set; }
    }
}