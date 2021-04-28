using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;

namespace UltraPro.Services.Interfaces
{
    public interface ISingleValueService : IDisposable
    {
        #region Single Value Detail
        Task<QueryResult<SingleValueDetail>> GetAllAsync(SingleValueQuery queryObj);
        Task<SingleValueDetail> GetByIdAsync(int id);
        Task<int> AddAsync(SingleValueDetail entity);
        Task<int> UpdateAsync(SingleValueDetail entity);
        Task<bool> ActiveInactiveAsync(int id);
        Task<bool> DeleteAsync(int id);
        Task<IList<SingleValueDetail>> GetAllActiveAsync(string typeCode);
        Task<IList<SingleValueDetail>> GetAllActiveAsync(int typeId);
        Task<IList<KeyValuePairObject>> GetAllForSelectAsync(int typeId);
        Task<IList<KeyValuePairObject>> GetAllForSelectAsync(string typeCode);
        Task<bool> IsExistsAsync(string name, int typeId, int id);
        #endregion

        #region Single Value Type
        Task<IList<SingleValueType>> GetAllActiveTypeAsync();
        Task<IList<KeyValuePairObject>> GetAllTypeForSelectAsync();
        #endregion
    }
}
