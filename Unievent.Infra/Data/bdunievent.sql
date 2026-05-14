CREATE TABLE responsavelevento(
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(200) NOT NULL,
    fotoPerfil VARCHAR(200)  -- FotoPerfil
);

CREATE TABLE endereco(
    id INT IDENTITY(1,1) PRIMARY KEY,
    rua VARCHAR(100) NOT NULL,       -- Rua
    cidade VARCHAR(80) NOT NULL,     -- Cidade
    bairro VARCHAR(100) NOT NULL,    -- Bairro
    estado CHAR(2) NOT NULL,         -- Estado
    cep VARCHAR(13) NOT NULL,        -- Cep
    numero VARCHAR(5) NOT NULL       -- Numero
);

CREATE TABLE instituicao(
    id INT IDENTITY(1,1) PRIMARY KEY,
    emailLogin VARCHAR(100) NOT NULL,     -- EmailLogin
    senhaLogin VARCHAR(60) NOT NULL,      -- SenhaLogin
    fotoPerfil VARCHAR(200) NOT NULL,     -- FotoPerfil
    cnpj VARCHAR(14) NOT NULL,            -- Cnpj
    idEndereco INT,                       -- IdEndereco
    CONSTRAINT FK_instituicao_endereco FOREIGN KEY(idEndereco)
        REFERENCES endereco(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE aluno(
    ra BIGINT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,           -- Nome
    senha VARCHAR(12) NOT NULL,           -- Senha
    email VARCHAR(100) NOT NULL,          -- Email  (era email_login)
    fotoPerfil VARCHAR(200),              -- FotoPerfil
    isAtivo BIT NOT NULL DEFAULT 0,       -- IsAtivo (campo novo)
    dataNascimento DATE NOT NULL,         -- DataNascimento
    idInstituicao INT,                    -- (FK, sem mapeamento direto na entidade)
    CONSTRAINT FK_aluno_instituicao FOREIGN KEY(idInstituicao)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE evento(
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(80) NOT NULL,                -- Nome
    descricao VARCHAR(150) NOT NULL,          -- Descricao
    categoria VARCHAR(40) NOT NULL,           -- Categoria (era categoria_evento)
    dataEvento DATE NOT NULL,                 -- DataEvento
    horaEvento TIME NOT NULL,                 -- HoraEvento
    capacidade INT NOT NULL,                  -- Capacidade
    thumbnail VARCHAR(400) NOT NULL,          -- Thumbnail[0]
    thumbnail2 VARCHAR(400),                  -- Thumbnail[1]
    thumbnail3 VARCHAR(400),                  -- Thumbnail[2]
    idResponsavelEvento INT,                  -- IdResponsavelEvento
    idEndereco INT,                           -- (sem mapeamento direto na entidade)
    CONSTRAINT FK_evento_responsavel FOREIGN KEY(idResponsavelEvento)
        REFERENCES responsavelevento(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_evento_endereco FOREIGN KEY(idEndereco)
        REFERENCES endereco(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE certificado(
    id INT IDENTITY(1,1) PRIMARY KEY,
    dataCertificado DATE NOT NULL,    -- DataCertificado (era data_certificado, corrige typo "DataCertifcado")
    texto VARCHAR(100) NOT NULL,      -- Texto
    idInstituicao INT,                -- (sem mapeamento direto na entidade Certificado)
    idAluno BIGINT,                   -- IdAluno
    idEvento INT,                     -- IdEvento
    CONSTRAINT FK_cert_instituicao FOREIGN KEY(idInstituicao)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_cert_aluno FOREIGN KEY(idAluno)
        REFERENCES aluno(ra)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_cert_evento FOREIGN KEY(idEvento)
        REFERENCES evento(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE instituicao_evento (
    id INT IDENTITY(1,1) PRIMARY KEY,
    idInstituicao INT NOT NULL,    -- IdInstituicao
    idEvento INT NOT NULL,         -- IdEvento
    CONSTRAINT UQ_inst_evento UNIQUE (idInstituicao, idEvento),
    CONSTRAINT FK_ie_instituicao FOREIGN KEY (idInstituicao)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_ie_evento FOREIGN KEY (idEvento)
        REFERENCES evento(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE secretaria(
    id INT IDENTITY(1,1) PRIMARY KEY,
    nomeUsuario VARCHAR(80) NOT NULL,         -- NomeUsuario
    emailUsuario VARCHAR(200) NOT NULL,       -- EmailUsuario
    senha VARCHAR(200) NOT NULL,              -- (sem mapeamento direto, mantido para autenticação)
    roleUsuario VARCHAR(50) NOT NULL,         -- RoleUsuario (era inexistente, adicionado)
    chave VARCHAR(300) UNIQUE,                -- Chave
    isAtivo BIT NOT NULL DEFAULT 0,           -- IsAtivo (substituiu situacao VARCHAR)
    tentativasLogin INT DEFAULT 0,            -- TentativasLogin
    idInstituicao INT,                        -- (FK, sem mapeamento direto na entidade)
    CONSTRAINT FK_secretaria_instituicao FOREIGN KEY (idInstituicao)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE log_evento (
    id INT IDENTITY(1,1) PRIMARY KEY,
    eventoId INT,                               -- eventoId (era evento_id)
    dataModificacao DATETIME DEFAULT GETDATE(), -- dataModificacao
    descricao VARCHAR(255),
    operacao VARCHAR(10)
);

GO
CREATE FUNCTION emailInstitucional (@email VARCHAR(100))
RETURNS BIT
AS
BEGIN
    DECLARE @email_validado BIT;

    IF @email LIKE '%@fatec.sp.gov.br'
        SET @email_validado = 1;
    ELSE
        SET @email_validado = 0;

    RETURN @email_validado;
END;
GO

CREATE TRIGGER log_evento_update
ON evento
AFTER UPDATE
AS
BEGIN
    INSERT INTO log_evento (eventoId, descricao, operacao)
    SELECT 
        i.id,
        'Evento atualizado: ' + d.nome + ' para ' + i.nome,
        'UPDATE'
    FROM inserted i
    JOIN deleted d ON i.id = d.id;
END;
GO

CREATE TRIGGER valida_desabilita_evento
ON evento
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM certificado c
        JOIN deleted d ON c.idEvento = d.id
    )
    BEGIN
        THROW 50000, 'Não é possível excluir este evento, pois existem certificados associados a ele.', 1;
    END

    DELETE FROM evento
    WHERE id IN (SELECT id FROM deleted);
END;
GO