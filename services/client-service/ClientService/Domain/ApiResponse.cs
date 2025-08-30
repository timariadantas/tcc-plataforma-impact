using System;

namespace ClientService.Domain
{
    public class ApiResponse<T>
    {
        public string Message { get; set; } = "";
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
        public long Elapsed { get; set; }
        public string? Error { get; set; }
        public T? Data { get; set; }
    }
}

//Anotações IPC 

//Armazena uma mensagem amigável sobre a resposta da API.Inicializada como string vazia.
//Armazena o momento em que a resposta foi criada.

//DateTime.UtcNow → pega o horário atual em UTC.
//.ToString("o") → formata a data no padrão ISO 8601 (2025-08-30T14:00:00.0000000Z), que é universal e legível por APIs.

// ELAPSED Tempo em milissegundos gasto para processar a requisição.
//Útil para monitoramento e logs de performance.

//Contém a descrição do erro, caso ocorra algum problema.
//string? → indica que pode ser null, ou seja, nem sempre terá valor.

// Contém o conteúdo real da resposta, que pode ser de qualquer tipo (T).
//Exemplo: um cliente, uma lista de produtos, ou um objeto de cotação.
//T? indica que a propriedade pode ser nula.