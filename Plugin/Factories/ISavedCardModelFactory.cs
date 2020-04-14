using BitShift.Plugin.Payments.FirstData.Domain;
using BitShift.Plugin.Payments.FirstData.Models;

namespace BitShift.Plugin.Payments.FirstData.Factories
{
    /// <summary>
    /// Represents the interface of the common models factory
    /// </summary>
    public partial interface ISavedCardModelFactory
    {
        /// <summary>
        /// Prepare the saved card model
        /// </summary>
        /// <returns>Saved card model</returns>
        SavedCardModel PrepareSavedCardModel(SavedCard card);
        
    }
}
