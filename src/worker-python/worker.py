import boto3
import json
import time
import psycopg  # Importando o psycopg (versão 3)

# Configurações do SQS e LocalStack
SQS_LOCALSTACK_URL = "http://localhost:4566"
QUEUE_URL = "http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/financial-operations-queue"

# Configurações do PostgreSQL
# No psycopg3, usamos uma connection string (URI) ou os parâmetros diretamente.
DB_CONNECTION_URI = "postgresql://admin:secretpassword@localhost:5434/financial_db"

sqs = boto3.client(
    "sqs",
    endpoint_url=SQS_LOCALSTACK_URL,
    region_name="us-east-1",
    aws_access_key_id="fake-access-key",
    aws_secret_access_key="fake-secret-key"
)

def update_transaction_status(transaction_id):
    """Atualiza o status da transação para Processed no PostgreSQL usando Psycopg 3"""
    try:
        # No psycopg3, usar o 'with' garante que a conexão fecha automaticamente
        with psycopg.connect(DB_CONNECTION_URI) as conn:
            with conn.cursor() as cursor:
                query = 'UPDATE "Transactions" SET "Status" = %s WHERE "Id" = %s'
                cursor.execute(query, ("Processed", transaction_id))
                conn.commit()
                print(f" -> Status da transação {transaction_id} atualizado para 'Processed' no banco.")
    except Exception as e:
        print(f"Erro ao atualizar o banco de dados: {e}")

print("Worker Python ativo e escutando a fila SQS...")

while True:
    try:
        response = sqs.receive_message(
            QueueUrl=QUEUE_URL,
            MaxNumberOfMessages=1,
            WaitTimeSeconds=5
        )

        messages = response.get("Messages", [])
        for message in messages:
            body = json.loads(message["Body"])
            
            # Garantindo a leitura mesmo com diferenças de maiúsculas/minúsculas
            transaction_id = body.get("Id") or body.get("id")
            asset_code = body.get("AssetCode") or body.get("assetCode")
            
            print(f"\n[Mensagem Recebida] Processando {asset_code} (ID: {transaction_id})...")

            # Simula um processamento rápido (ex: validação)
            time.sleep(1)

            # 1. Atualiza o status no PostgreSQL
            if transaction_id:
                update_transaction_status(transaction_id)

            # 2. Remove a mensagem da fila SQS
            sqs.delete_message(
                QueueUrl=QUEUE_URL,
                ReceiptHandle=message["ReceiptHandle"]
            )
            print(f" -> Mensagem {transaction_id} removida da fila SQS.")

    except Exception as e:
        print(f"Erro ao processar mensagem: {e}")

    time.sleep(1)