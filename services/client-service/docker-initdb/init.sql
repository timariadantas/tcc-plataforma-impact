-- Habilita extensão para gerar valores aleatórios (necessário para ULID/hex)
CREATE EXTENSION IF NOT EXISTS "pgcrypto";



CREATE TABLE IF NOT EXISTS clients (
    id TEXT PRIMARY KEY DEFAULT encode(gen_random_bytes(16), 'hex'), -- ULID-like,
    name TEXT NOT NULL,
    surname TEXT NOT NULL,
    email TEXT NOT NULL,
    birthdate DATE NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    active BOOLEAN NOT NULL DEFAULT true
);

-- Alterar coluna 'id' para gerar valor automaticamente se não informado
ALTER TABLE clients
ALTER COLUMN id SET DEFAULT encode(gen_random_bytes(16), 'hex');

-- Definir default para created_at e updated_at
ALTER TABLE clients
ALTER COLUMN created_at SET DEFAULT NOW();
ALTER TABLE clients
ALTER COLUMN updated_at SET DEFAULT NOW();

-- Definir default para active
ALTER TABLE clients
ALTER COLUMN active SET DEFAULT true;


-- índices úteis
CREATE INDEX IF NOT EXISTS idx_clients_email ON clients(email);
CREATE INDEX IF NOT EXISTS idx_clients_active ON clients(active);

-- atualizar sozinho toda vez que o registro muda
-- Função e trigger para updated_at
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
   NEW.updated_at = NOW();
   RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_updated_at
BEFORE UPDATE ON clients
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();
