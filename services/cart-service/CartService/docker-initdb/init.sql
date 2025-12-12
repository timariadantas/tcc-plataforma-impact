

-- Criar tabela sales
CREATE TABLE IF NOT EXISTS sales (
    id TEXT PRIMARY KEY ,
    client_id TEXT NOT NULL,
    status INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Criar tabela sale_items
CREATE TABLE IF NOT EXISTS sale_items (
    id TEXT PRIMARY KEY ,
    sell_id TEXT NOT NULL REFERENCES sales(id) ON DELETE CASCADE,  -- relacionamento de 1-para-muitos /ON DELETE CASCADE exclui automaticamente os itens quando a venda é excluída.
    product_id TEXT NOT NULL,
    quantity INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Trigger para atualizar updated_at automaticamente
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
   NEW.updated_at = NOW();
   RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_updated_at_sales
BEFORE UPDATE ON sales
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER set_updated_at_sale_items
BEFORE UPDATE ON sale_items
FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();

-- Índices úteis
CREATE INDEX IF NOT EXISTS idx_sales_client_id ON sales(client_id);
CREATE INDEX IF NOT EXISTS idx_sale_items_sell_id ON sale_items(sell_id);
