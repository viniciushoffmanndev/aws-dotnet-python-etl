import boto3
import json
import time

# Configura o cliente SQS apontando para o LocalStack
sqs = boto3.client(
    'sqs',
    endpoint_url='http://localhost:4566',
    region_name='us-east-1',
    aws_access_key_id='fake-access-key',
    aws_secret_access_key='fake-secret-key'
)

QUEUE_URL = 'http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/financial-operations-queue'

def process_message(message_body):
    """
    Função que simula o processamento inteligente dos dados da transação.
    """
    try:
        data = json.loads(message_body)
        print("\n--- Nova Transação Detectada pelo Worker Python ---")
        print(f"ID: {data.get('Id')}")
        print(f"Ativo: {data.get('AssetCode')}")
        print(f"Emissor: {data.get('Issuer')}")
        print(f"Valor: R$ {data.get('Price')}")
        print(f"Data de Criação: {data.get('CreatedAt')}")
        print("---------------------------------------------------\n")
        return True
    except Exception as e:
        print(f"Erro ao processar mensagem: {e}")
        return False

def listen_queue():
    print(f"Worker Python ativo e escutando a fila SQS...")
    
    while True:
        try:
            # Busca mensagens na fila do SQS
            response = sqs.receive_message(
                QueueUrl=QUEUE_URL,
                MaxNumberOfMessages=1,
                WaitTimeSeconds=5  # Long polling para evitar requisições excessivas
            )
            
            messages = response.get('Messages', [])
            
            if not messages:
                # Nenhuma mensagem na fila, continua escutando
                continue
                
            for message in messages:
                receipt_handle = message['ReceiptHandle']
                body = message['Body']
                
                # Executa o processamento
                success = process_message(body)
                
                # Se processou com sucesso, apaga a mensagem da fila para não duplicar
                if success:
                    sqs.delete_message(
                        QueueUrl=QUEUE_URL,
                        ReceiptHandle=receipt_handle
                    )
                    print("[Sucesso] Mensagem processada e removida da fila.")
                    
        except Exception as e:
            print(f"Erro ao escutar a fila: {e}")
            time.sleep(5)

if __name__ == '__main__':
    listen_queue()