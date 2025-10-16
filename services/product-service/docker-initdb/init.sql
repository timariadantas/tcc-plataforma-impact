-- Habilita extensão para gerar valores aleatórios (necessário para ULID/hex)
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Criar tabela products
CREATE TABLE IF NOT EXISTS products (
    id TEXT PRIMARY KEY DEFAULT encode(gen_random_bytes(16), 'hex'), -- ULID-like
    name TEXT NOT NULL,
    description TEXT,
    price REAL NOT NULL,
    quantity INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
    active BOOLEAN NOT NULL DEFAULT TRUE
);



-- Alterar coluna 'id' para gerar valor automaticamente se não informado
ALTER TABLE products
ALTER COLUMN id SET DEFAULT encode(gen_random_bytes(16), 'hex');

-- Definir default para created_at e updated_at
ALTER TABLE products
ALTER COLUMN created_at SET DEFAULT NOW();
ALTER TABLE products
ALTER COLUMN updated_at SET DEFAULT NOW();

-- Definir default para active
ALTER TABLE products
ALTER COLUMN active SET DEFAULT TRUE;

ALTER TABLE products
ALTER COLUMN price TYPE NUMERIC(10,2) USING price::NUMERIC;





-- índices úteis
CREATE INDEX IF NOT EXISTS idx_products_name ON products(name);
CREATE INDEX IF NOT EXISTS idx_products_active ON products(active);

-- Função e trigger para atualizar updated_at automaticamente
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
   NEW.updated_at = NOW();
   RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_updated_at
BEFORE UPDATE ON products
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();
