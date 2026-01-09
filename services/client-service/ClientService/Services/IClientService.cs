using ClientService.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientService.Service
{
    public interface IClientService
    {
        Client Create(string name, string surname, string email, DateTime birthdate);                
        Client?GetById(string id);         
        List<Client>GetAll();                             
        void Update(string name, string surname, string email, DateTime birthdate);
        void  Delete(string id);   
    void Deactivate(string id);
 
    }
}
