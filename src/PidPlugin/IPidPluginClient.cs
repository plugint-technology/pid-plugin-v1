using System.Threading;
using System.Threading.Tasks;
using PidPlugin.Dtos;

namespace PidPlugin
{
    public interface IPidPluginClient
    {
        Task<EntityBasicData>       GetEntityDataBasicAsync      (string cuit, CancellationToken cancellationToken = default);
        Task<EntityFullData>        GetEntityDataFullAsync       (string cuit, CancellationToken cancellationToken = default);
        Task<SpecialRecordEntry>    GetSpecialRecordsAsync       (string cuit, string rule, CancellationToken cancellationToken = default);
        Task<BankAccountDetail>     GetBankAccountDetailAsync    (string cbu, CancellationToken cancellationToken = default);
        Task<BankAccountOwner>      GetBankAccountOwnershipAsync (string cbu, string cuit, CancellationToken cancellationToken = default);
    }
}
