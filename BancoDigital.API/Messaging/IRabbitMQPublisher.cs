namespace BancoDigital.API.Messaging;

public interface IRabbitMQPublisher
{
    void Publicar(int contratacaoId);
}
