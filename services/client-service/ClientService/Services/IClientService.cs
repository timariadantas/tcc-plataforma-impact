using ClientService.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientService.Service
{
    public interface IClientService
    {
        void  Create(Client client);                
        Client?GetById(string id);         
        List<Client>GetAll();              
        void  Delete(string id);                
        void Update(Client client);
    
    }
}
