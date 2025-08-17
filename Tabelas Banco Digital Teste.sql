CREATE TABLE contacorrente (
	idcontacorrente CHAR(36) PRIMARY KEY, -- id da conta corrente
	numero INTEGER(10) NOT NULL UNIQUE, -- numero da conta corrente
	nome TEXT(100) NOT NULL, -- nome do titular da conta corrente
	ativo INTEGER(1) NOT NULL default 0, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
	senha TEXT(100) NOT NULL,
	salt TEXT(100) NOT NULL,
    saldo DECIMAL(10, 2),
	CHECK (ativo in (0,1)),
    cpf TEXT(11)
);

CREATE TABLE movimento (
	idmovimento CHAR(36) PRIMARY KEY, -- identificacao unica do movimento
	idcontacorrente CHAR(36) NOT NULL, -- identificacao unica da conta corrente
	datamovimento TEXT(25) NOT NULL, -- data do movimento no formato DD/MM/YYYY
	tipomovimento TEXT(1) NOT NULL, -- tipo do movimento. (C = Credito, D = Debito).
	valor DECIMAL(10, 2) NOT NULL, -- valor do movimento. Usar duas casas decimais.
	CHECK (tipomovimento in ('C','D')),
	FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
);

CREATE TABLE idempotencia (
	chave_idempotencia CHAR(36) PRIMARY KEY, -- identificacao chave de idempotencia
	requisicao TEXT(1000), -- dados de requisicao
	resultado TEXT(1000) -- dados de retorno
);

CREATE TABLE tarifa (
	idtarifa CHAR(36) PRIMARY KEY, -- identificacao unica da tarifa
	idcontacorrente CHAR(36) NOT NULL, -- identificacao unica da conta corrente
	datamovimento TEXT(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
	valor DECIMAL(10, 2) NOT NULL -- valor da tarifa. Usar duas casas decimais.
);

CREATE TABLE transferencia (
	idtransferencia CHAR(36) PRIMARY KEY, -- identificacao unica da transferencia
	idcontacorrente_origem CHAR(36) NOT NULL, -- identificacao unica da conta corrente de origem
	idcontacorrente_destino CHAR(36) NOT NULL, -- identificacao unica da conta corrente de destino
	datamovimento TEXT(25) NOT NULL, -- data do transferencia no formato DD/MM/YYYY
	valor DECIMAL(10, 2) NOT NULL-- valor da transferencia. Usar duas casas decimais.
);