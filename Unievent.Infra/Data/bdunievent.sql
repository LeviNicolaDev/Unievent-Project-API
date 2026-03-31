/*
CREATE DATABASE bdunievent;
USE bdunievent;
*/

CREATE TABLE responsavelevento(
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(200) NOT NULL,
    fotoPerfil VARCHAR(200)
);

CREATE TABLE endereco(
    id INT IDENTITY(1,1) PRIMARY KEY,
    rua VARCHAR(100) NOT NULL,
    cidade VARCHAR(80) NOT NULL,
    bairro VARCHAR(100) NOT NULL,
    estado CHAR(2) NOT NULL,
    cep VARCHAR(13) NOT NULL,
    numero VARCHAR(5) NOT NULL
);

CREATE TABLE instituicao(
    id INT IDENTITY(1,1) PRIMARY KEY,
    email_login VARCHAR(100) NOT NULL,
    senha_login VARCHAR(60) NOT NULL,
    foto_perfil VARCHAR(200) NOT NULL,
    cnpj VARCHAR(14) NOT NULL,
    id_endereco_fk INT,
    CONSTRAINT FK_instituicao_endereco FOREIGN KEY(id_endereco_fk)
        REFERENCES endereco(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE aluno(
    ra BIGINT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    senha VARCHAR(12) NOT NULL,
    email_login VARCHAR(100) NOT NULL,
    foto_perfil VARCHAR(200),
    data_nascimento DATE NOT NULL,
    id_instituicao_fk INT,
    CONSTRAINT FK_aluno_instituicao FOREIGN KEY(id_instituicao_fk)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE evento(
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(80) NOT NULL,
    descricao VARCHAR(150) NOT NULL,
    categoria_evento VARCHAR(40) NOT NULL,
    data_evento DATE NOT NULL,
    hora_evento TIME NOT NULL,
    capacidade INT NOT NULL,
    thumbnail VARCHAR(400) NOT NULL,
    thumbnail2 VARCHAR(400),
    thumbnail3 VARCHAR(400),
    id_responsavel_evento_fk INT,
    id_endereco_fk INT,
    CONSTRAINT FK_evento_responsavel FOREIGN KEY(id_responsavel_evento_fk)
        REFERENCES responsavelevento(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_evento_endereco FOREIGN KEY(id_endereco_fk)
        REFERENCES endereco(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE certificado(
    id INT IDENTITY(1,1) PRIMARY KEY,
    data_certificado DATE NOT NULL,
    texto VARCHAR(100) NOT NULL,
    id_instituicao_fk INT,
    id_aluno_fk BIGINT,
    id_evento_fk INT,
    CONSTRAINT FK_cert_instituicao FOREIGN KEY(id_instituicao_fk)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_cert_aluno FOREIGN KEY(id_aluno_fk)
        REFERENCES aluno(ra)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_cert_evento FOREIGN KEY(id_evento_fk)
        REFERENCES evento(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE instituicao_evento (
    id INT IDENTITY(1,1) PRIMARY KEY,
    id_instituicao_fk INT NOT NULL,
    id_evento_fk INT NOT NULL,
    CONSTRAINT UQ_inst_evento UNIQUE (id_instituicao_fk, id_evento_fk),
    CONSTRAINT FK_ie_instituicao FOREIGN KEY (id_instituicao_fk)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT FK_ie_evento FOREIGN KEY (id_evento_fk)
        REFERENCES evento(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE secretaria(
    id INT IDENTITY(1,1) PRIMARY KEY,
    nome VARCHAR(80) NOT NULL,
    email VARCHAR(200) NOT NULL,
    senha VARCHAR(200) NOT NULL,
    chave VARCHAR(300) UNIQUE,
    situacao VARCHAR(50) DEFAULT 'inativo',
    tentativas_login INT DEFAULT 0,
    id_instituicao_fk INT,
    CONSTRAINT FK_secretaria_instituicao FOREIGN KEY (id_instituicao_fk)
        REFERENCES instituicao(id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION
);

CREATE TABLE log_evento (
    id INT IDENTITY(1,1) PRIMARY KEY,
    evento_id INT,
    data_modificacao DATETIME DEFAULT GETDATE(),
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
    INSERT INTO log_evento (evento_id, descricao, operacao)
    SELECT 
        i.id,
        'Evento atualizado: ' + d.nome + ' para ' + i.nome,
        'UPDATE'
    FROM inserted i
    JOIN deleted d ON i.id = d.id;
END;
GO

GO
CREATE TRIGGER valida_desabilita_evento
ON evento
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (
        SELECT 1
        FROM certificado c
        JOIN deleted d ON c.id_evento_fk = d.id
    )
    BEGIN
        THROW 50000, 'Não é possível excluir este evento, pois existem certificados associados a ele.', 1;
    END

    DELETE FROM evento
    WHERE id IN (SELECT id FROM deleted);
END;
GO