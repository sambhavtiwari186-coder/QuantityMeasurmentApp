using QuantityMeasurementApp.Entity.DTO;
using System.Threading.Tasks;

namespace QuantityMeasurementApp.Service.Interface
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
    }
}
