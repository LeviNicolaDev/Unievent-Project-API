IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE TABLE [Aluno] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(max) NOT NULL,
        [Senha] nvarchar(max) NOT NULL,
        [Email] nvarchar(450) NOT NULL,
        [FotoPerfil] nvarchar(max) NOT NULL,
        [IsAtivo] bit NOT NULL,
        [DataNascimento] datetime2 NOT NULL,
        CONSTRAINT [PK_Aluno] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE TABLE [Endereco] (
        [Id] int NOT NULL IDENTITY,
        [Rua] nvarchar(max) NOT NULL,
        [Cidade] nvarchar(max) NOT NULL,
        [Bairro] nvarchar(max) NOT NULL,
        [Estado] nvarchar(2) NOT NULL,
        [Cep] nvarchar(8) NOT NULL,
        [Numero] nvarchar(5) NOT NULL,
        CONSTRAINT [PK_Endereco] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE TABLE [ResponsavelEvento] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(max) NOT NULL,
        [FotoPerfil] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_ResponsavelEvento] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE TABLE [UsuarioSecretaria] (
        [Id] int NOT NULL IDENTITY,
        [NomeUsuario] nvarchar(max) NOT NULL,
        [RoleUsuario] int NOT NULL,
        [EmailUsuario] nvarchar(450) NOT NULL,
        [Chave] nvarchar(300) NOT NULL,
        [TentativasLogin] int NOT NULL,
        [IsAtivo] bit NOT NULL,
        CONSTRAINT [PK_UsuarioSecretaria] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE TABLE [Instituicao] (
        [Id] int NOT NULL IDENTITY,
        [EmailLogin] nvarchar(max) NOT NULL,
        [SenhaLogin] nvarchar(max) NOT NULL,
        [FotoPerfil] nvarchar(max) NOT NULL,
        [Cnpj] nvarchar(14) NOT NULL,
        [IdEndereco] int NOT NULL,
        [EnderecoId] int NOT NULL,
        CONSTRAINT [PK_Instituicao] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Instituicao_Endereco_EnderecoId] FOREIGN KEY ([EnderecoId]) REFERENCES [Endereco] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE TABLE [Evento] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(max) NOT NULL,
        [Descricao] nvarchar(max) NOT NULL,
        [Categoria] nvarchar(max) NOT NULL,
        [HoraEvento] nvarchar(max) NOT NULL,
        [DataEvento] datetime2 NOT NULL,
        [IdResponsavelEvento] int NOT NULL,
        [ResponsavelEventoId] int NOT NULL,
        [Capacidade] int NOT NULL,
        [Thumbnail] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Evento] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Evento_ResponsavelEvento_ResponsavelEventoId] FOREIGN KEY ([ResponsavelEventoId]) REFERENCES [ResponsavelEvento] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE TABLE [Certificado] (
        [Id] int NOT NULL IDENTITY,
        [DataCertifcado] datetime2 NOT NULL,
        [Texto] nvarchar(max) NOT NULL,
        [AlunoId] int NOT NULL,
        [IdAluno] int NOT NULL,
        [EventoId] int NOT NULL,
        [IdEvento] int NOT NULL,
        CONSTRAINT [PK_Certificado] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Certificado_Aluno_AlunoId] FOREIGN KEY ([AlunoId]) REFERENCES [Aluno] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Certificado_Evento_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [Evento] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Aluno_Email] ON [Aluno] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE INDEX [IX_Certificado_AlunoId] ON [Certificado] ([AlunoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE INDEX [IX_Certificado_EventoId] ON [Certificado] ([EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE INDEX [IX_Evento_ResponsavelEventoId] ON [Evento] ([ResponsavelEventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE INDEX [IX_Instituicao_EnderecoId] ON [Instituicao] ([EnderecoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UsuarioSecretaria_Chave] ON [UsuarioSecretaria] ([Chave]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UsuarioSecretaria_EmailUsuario] ON [UsuarioSecretaria] ([EmailUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407144806_First'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260407144806_First', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407155007_Fix Entity Evento'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Evento]') AND [c].[name] = N'IdResponsavelEvento');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Evento] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Evento] DROP COLUMN [IdResponsavelEvento];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407155007_Fix Entity Evento'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260407155007_Fix Entity Evento', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407161143_FixCnpjSize'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Instituicao]') AND [c].[name] = N'IdEndereco');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Instituicao] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Instituicao] DROP COLUMN [IdEndereco];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407161143_FixCnpjSize'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Instituicao]') AND [c].[name] = N'Cnpj');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Instituicao] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [Instituicao] ALTER COLUMN [Cnpj] nvarchar(18) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407161143_FixCnpjSize'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260407161143_FixCnpjSize', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407195737_Add campo ''Senha'' na tabela UsuarioSecretaria'
)
BEGIN
    ALTER TABLE [UsuarioSecretaria] ADD [Senha] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260407195737_Add campo ''Senha'' na tabela UsuarioSecretaria'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260407195737_Add campo ''Senha'' na tabela UsuarioSecretaria', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408135141_arrumando colunas fk da entidade Certififcado'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Certificado]') AND [c].[name] = N'IdAluno');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Certificado] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Certificado] DROP COLUMN [IdAluno];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408135141_arrumando colunas fk da entidade Certififcado'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Certificado]') AND [c].[name] = N'IdEvento');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Certificado] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Certificado] DROP COLUMN [IdEvento];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260408135141_arrumando colunas fk da entidade Certififcado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260408135141_arrumando colunas fk da entidade Certififcado', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514153955_fix column entity event'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Evento]') AND [c].[name] = N'HoraEvento');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Evento] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [Evento] DROP COLUMN [HoraEvento];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260514153955_fix column entity event'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260514153955_fix column entity event', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519205206_inital'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519205206_inital', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260526124036_initial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260526124036_initial', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260526125414_fix: conserto da coluna RoleUsuario na tabela UsuarioSecretaria'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UsuarioSecretaria]') AND [c].[name] = N'RoleUsuario');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [UsuarioSecretaria] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [UsuarioSecretaria] ALTER COLUMN [RoleUsuario] nvarchar(max) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260526125414_fix: conserto da coluna RoleUsuario na tabela UsuarioSecretaria'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Instituicao_Cnpj] ON [Instituicao] ([Cnpj]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260526125414_fix: conserto da coluna RoleUsuario na tabela UsuarioSecretaria'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260526125414_fix: conserto da coluna RoleUsuario na tabela UsuarioSecretaria', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527225403_add nova tabela para inscrever se em evento'
)
BEGIN
    CREATE TABLE [Participacao] (
        [Id] int NOT NULL IDENTITY,
        [AlunoId] int NOT NULL,
        [EventoId] int NOT NULL,
        [PresencaConfirmada] bit NOT NULL,
        [DataConfirmacao] datetime2 NULL,
        [CertificadoEmitido] bit NOT NULL,
        [CodigoValidacao] nvarchar(max) NULL,
        CONSTRAINT [PK_Participacao] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Participacao_Aluno_AlunoId] FOREIGN KEY ([AlunoId]) REFERENCES [Aluno] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Participacao_Evento_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [Evento] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527225403_add nova tabela para inscrever se em evento'
)
BEGIN
    CREATE INDEX [IX_Participacao_AlunoId] ON [Participacao] ([AlunoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527225403_add nova tabela para inscrever se em evento'
)
BEGIN
    CREATE INDEX [IX_Participacao_EventoId] ON [Participacao] ([EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527225403_add nova tabela para inscrever se em evento'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260527225403_add nova tabela para inscrever se em evento', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527234534_fix tabela aluno'
)
BEGIN
    ALTER TABLE [Aluno] ADD [Role] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260527234534_fix tabela aluno'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260527234534_fix tabela aluno', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260608151407_fix table certificado'
)
BEGIN
    ALTER TABLE [Certificado] DROP CONSTRAINT [FK_Certificado_Aluno_AlunoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260608151407_fix table certificado'
)
BEGIN
    DROP INDEX [IX_Certificado_AlunoId] ON [Certificado];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260608151407_fix table certificado'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Certificado]') AND [c].[name] = N'AlunoId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Certificado] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [Certificado] DROP COLUMN [AlunoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260608151407_fix table certificado'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260608151407_fix table certificado', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    DROP INDEX [IX_Participacao_AlunoId] ON [Participacao];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Participacao] ADD [CodigoIngresso] nvarchar(450) NOT NULL DEFAULT (LOWER(REPLACE(CONVERT(varchar(36), NEWID()), '-', '')));
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Participacao] ADD [DistanciaCheckInMetros] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Participacao] ADD [OperadorCheckInId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Participacao] ADD [PrecisaoLocalizacaoMetros] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [EnderecoId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [FimInscricoes] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [InicioInscricoes] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [InstituicaoId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [Latitude] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [Longitude] float NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [PublicoPermitido] nvarchar(max) NOT NULL DEFAULT N'Todos';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [RaioCheckInMetros] int NOT NULL DEFAULT 150;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [ToleranciaCheckInMinutos] int NOT NULL DEFAULT 60;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD [Visibilidade] nvarchar(max) NOT NULL DEFAULT N'Publico';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Aluno] ADD [InstituicaoId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Aluno] ADD [TipoParticipante] nvarchar(max) NOT NULL DEFAULT N'Interno';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    CREATE TABLE [PreferenciaNotificacao] (
        [Id] int NOT NULL IDENTITY,
        [AlunoId] int NOT NULL,
        [LembretesEventos] bit NOT NULL,
        [AlertasCertificados] bit NOT NULL,
        [Recomendacoes] bit NOT NULL,
        [UsarLocalizacao] bit NOT NULL,
        [Categorias] nvarchar(max) NULL,
        [LatitudeAproximada] float NULL,
        [LongitudeAproximada] float NULL,
        [RaioKm] int NOT NULL,
        [AtualizadoEmUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_PreferenciaNotificacao] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PreferenciaNotificacao_Aluno_AlunoId] FOREIGN KEY ([AlunoId]) REFERENCES [Aluno] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Participacao_AlunoId_EventoId] ON [Participacao] ([AlunoId], [EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Participacao_CodigoIngresso] ON [Participacao] ([CodigoIngresso]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    CREATE INDEX [IX_Evento_EnderecoId] ON [Evento] ([EnderecoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    CREATE INDEX [IX_Evento_InstituicaoId] ON [Evento] ([InstituicaoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    CREATE INDEX [IX_Aluno_InstituicaoId] ON [Aluno] ([InstituicaoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PreferenciaNotificacao_AlunoId] ON [PreferenciaNotificacao] ([AlunoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Aluno] ADD CONSTRAINT [FK_Aluno_Instituicao_InstituicaoId] FOREIGN KEY ([InstituicaoId]) REFERENCES [Instituicao] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD CONSTRAINT [FK_Evento_Endereco_EnderecoId] FOREIGN KEY ([EnderecoId]) REFERENCES [Endereco] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    ALTER TABLE [Evento] ADD CONSTRAINT [FK_Evento_Instituicao_InstituicaoId] FOREIGN KEY ([InstituicaoId]) REFERENCES [Instituicao] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819122603_AddAutomacoesERegrasEvento'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819122603_AddAutomacoesERegrasEvento', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819123207_AddNotificacaoEvento'
)
BEGIN
    CREATE TABLE [NotificacaoEvento] (
        [Id] int NOT NULL IDENTITY,
        [AlunoId] int NOT NULL,
        [EventoId] int NOT NULL,
        [Tipo] nvarchar(60) NOT NULL,
        [Assunto] nvarchar(200) NOT NULL,
        [Mensagem] nvarchar(max) NOT NULL,
        [ProcessadaEmUtc] datetime2 NOT NULL,
        [Enviada] bit NOT NULL,
        [Erro] nvarchar(max) NULL,
        CONSTRAINT [PK_NotificacaoEvento] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NotificacaoEvento_Aluno_AlunoId] FOREIGN KEY ([AlunoId]) REFERENCES [Aluno] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_NotificacaoEvento_Evento_EventoId] FOREIGN KEY ([EventoId]) REFERENCES [Evento] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819123207_AddNotificacaoEvento'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NotificacaoEvento_AlunoId_EventoId_Tipo] ON [NotificacaoEvento] ([AlunoId], [EventoId], [Tipo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819123207_AddNotificacaoEvento'
)
BEGIN
    CREATE INDEX [IX_NotificacaoEvento_EventoId] ON [NotificacaoEvento] ([EventoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819123207_AddNotificacaoEvento'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819123207_AddNotificacaoEvento', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819125148_novas_tabelas'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819125148_novas_tabelas', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    DROP INDEX [IX_Evento_InstituicaoId] ON [Evento];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [UsuarioSecretaria] ADD [InstituicaoId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [ResponsavelEvento] ADD [InstituicaoId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [AtualizadoEmUtc] datetime2 NOT NULL DEFAULT (GETUTCDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Codigo] nvarchar(60) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [CriadoEmUtc] datetime2 NOT NULL DEFAULT (GETUTCDATE());
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [IsAtivo] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Nome] nvarchar(160) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [NomeAbreviado] nvarchar(40) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Site] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Telefone] nvarchar(30) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Evento]') AND [c].[name] = N'Visibilidade');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Evento] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [Evento] ALTER COLUMN [Visibilidade] nvarchar(450) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Evento]') AND [c].[name] = N'PublicoPermitido');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Evento] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [Evento] ALTER COLUMN [PublicoPermitido] nvarchar(450) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Evento]') AND [c].[name] = N'Categoria');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Evento] DROP CONSTRAINT ' + @var10 + ';');
    ALTER TABLE [Evento] ALTER COLUMN [Categoria] nvarchar(450) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    CREATE INDEX [IX_UsuarioSecretaria_InstituicaoId] ON [UsuarioSecretaria] ([InstituicaoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    CREATE INDEX [IX_ResponsavelEvento_InstituicaoId] ON [ResponsavelEvento] ([InstituicaoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Instituicao_Codigo] ON [Instituicao] ([Codigo]) WHERE [Codigo] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    CREATE INDEX [IX_Evento_InstituicaoId_Categoria] ON [Evento] ([InstituicaoId], [Categoria]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    CREATE INDEX [IX_Evento_InstituicaoId_DataEvento] ON [Evento] ([InstituicaoId], [DataEvento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    CREATE INDEX [IX_Evento_Visibilidade_PublicoPermitido] ON [Evento] ([Visibilidade], [PublicoPermitido]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [ResponsavelEvento] ADD CONSTRAINT [FK_ResponsavelEvento_Instituicao_InstituicaoId] FOREIGN KEY ([InstituicaoId]) REFERENCES [Instituicao] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    ALTER TABLE [UsuarioSecretaria] ADD CONSTRAINT [FK_UsuarioSecretaria_Instituicao_InstituicaoId] FOREIGN KEY ([InstituicaoId]) REFERENCES [Instituicao] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819133754_AddMultiInstituicao'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819133754_AddMultiInstituicao', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820141910_AddUsuarioUnievent'
)
BEGIN
    CREATE TABLE [UsuarioUnievent] (
        [Id] int NOT NULL IDENTITY,
        [NomeUsuario] nvarchar(max) NOT NULL,
        [EmailUsuario] nvarchar(450) NOT NULL,
        [Senha] nvarchar(max) NOT NULL,
        [Chave] nvarchar(300) NOT NULL,
        [RoleUsuario] nvarchar(max) NOT NULL,
        [TentativasLogin] int NOT NULL,
        [IsAtivo] bit NOT NULL,
        [CriadoEmUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_UsuarioUnievent] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820141910_AddUsuarioUnievent'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UsuarioUnievent_Chave] ON [UsuarioUnievent] ([Chave]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820141910_AddUsuarioUnievent'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UsuarioUnievent_EmailUsuario] ON [UsuarioUnievent] ([EmailUsuario]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820141910_AddUsuarioUnievent'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260820141910_AddUsuarioUnievent', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Evento] DROP CONSTRAINT [FK_Evento_Endereco_EnderecoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] DROP CONSTRAINT [FK_Instituicao_Endereco_EnderecoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Bairro] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Cep] nvarchar(8) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Cidade] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Estado] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Numero] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    ALTER TABLE [Instituicao] ADD [Rua] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    UPDATE i
    SET
        i.Bairro = LEFT(COALESCE(e.Bairro, ''), 100),
        i.Cep = LEFT(COALESCE(e.Cep, ''), 8),
        i.Cidade = LEFT(COALESCE(e.Cidade, ''), 100),
        i.Estado = LEFT(COALESCE(e.Estado, ''), 50),
        i.Numero = LEFT(COALESCE(e.Numero, ''), 20),
        i.Rua = LEFT(COALESCE(e.Rua, ''), 200)
    FROM Instituicao i
    LEFT JOIN Endereco e ON e.Id = i.EnderecoId;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    DROP INDEX [IX_Instituicao_EnderecoId] ON [Instituicao];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    DROP INDEX [IX_Evento_EnderecoId] ON [Evento];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Instituicao]') AND [c].[name] = N'EnderecoId');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Instituicao] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [Instituicao] DROP COLUMN [EnderecoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Evento]') AND [c].[name] = N'EnderecoId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Evento] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [Evento] DROP COLUMN [EnderecoId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    DROP TABLE [Endereco];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820170954_MoveEnderecoFieldsToInstituicao'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260820170954_MoveEnderecoFieldsToInstituicao', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820173953_RemoveCredenciaisInstituicao'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Instituicao]') AND [c].[name] = N'EmailLogin');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Instituicao] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [Instituicao] DROP COLUMN [EmailLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820173953_RemoveCredenciaisInstituicao'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Instituicao]') AND [c].[name] = N'SenhaLogin');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Instituicao] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [Instituicao] DROP COLUMN [SenhaLogin];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820173953_RemoveCredenciaisInstituicao'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260820173953_RemoveCredenciaisInstituicao', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820181245_AddStatusUsuarioSecretaria'
)
BEGIN
    ALTER TABLE [UsuarioSecretaria] ADD [Status] nvarchar(max) NOT NULL DEFAULT N'Ativo';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820181245_AddStatusUsuarioSecretaria'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260820181245_AddStatusUsuarioSecretaria', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820201240_AddEmailConfirmation'
)
BEGIN
    ALTER TABLE [UsuarioSecretaria] ADD [EmailConfirmado] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820201240_AddEmailConfirmation'
)
BEGIN
    ALTER TABLE [Aluno] ADD [ChaveConfirmacaoEmail] nvarchar(300) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820201240_AddEmailConfirmation'
)
BEGIN
    ALTER TABLE [Aluno] ADD [EmailConfirmado] bit NOT NULL DEFAULT CAST(1 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820201240_AddEmailConfirmation'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Aluno_ChaveConfirmacaoEmail] ON [Aluno] ([ChaveConfirmacaoEmail]) WHERE [ChaveConfirmacaoEmail] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260820201240_AddEmailConfirmation'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260820201240_AddEmailConfirmation', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821113028_AddEventoLocal'
)
BEGIN
    ALTER TABLE [Evento] ADD [Local] nvarchar(200) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821113028_AddEventoLocal'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260821113028_AddEventoLocal', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821140715_SimplifyEventoPublicoPermitido'
)
BEGIN
    UPDATE [Evento]
    SET [PublicoPermitido] = N'PublicoGeral'
    WHERE [PublicoPermitido] IN (N'Todos', N'SomenteExternos')
       OR [PublicoPermitido] IS NULL;

    UPDATE [Evento]
    SET [PublicoPermitido] = N'TodosAlunosFatec'
    WHERE [PublicoPermitido] = N'SomenteInternos';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821140715_SimplifyEventoPublicoPermitido'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260821140715_SimplifyEventoPublicoPermitido', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821144821_AddCertificadoEmailTracking'
)
BEGIN
    ALTER TABLE [Participacao] ADD [CertificadoEnviadoPorEmail] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821144821_AddCertificadoEmailTracking'
)
BEGIN
    ALTER TABLE [Participacao] ADD [DataEnvioCertificadoEmail] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821144821_AddCertificadoEmailTracking'
)
BEGIN
    ALTER TABLE [Participacao] ADD [ErroEnvioCertificadoEmail] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260821144821_AddCertificadoEmailTracking'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260821144821_AddCertificadoEmailTracking', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [CertificadoPdf] varbinary(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [DataGeracaoCertificado] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [DestinatarioCertificadoEmail] nvarchar(320) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [NomeArquivoCertificado] nvarchar(180) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [ProcessamentoCertificadoAteUtc] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [ProcessamentoCertificadoId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [ProximaTentativaCertificadoUtc] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [StatusEnvioCertificado] nvarchar(30) NOT NULL DEFAULT N'Pendente';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    ALTER TABLE [Participacao] ADD [TentativasEnvioCertificado] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    UPDATE [Participacao]
    SET [StatusEnvioCertificado] = CASE
        WHEN [CertificadoEnviadoPorEmail] = 1 THEN 'Enviado'
        WHEN [CertificadoEmitido] = 1 THEN 'EnvioIncerto'
        ELSE 'Pendente' END;
    UPDATE [Participacao]
    SET [ErroEnvioCertificadoEmail] = N'Emissão anterior sem confirmação de entrega; verificar no provedor antes de reenviar.'
    WHERE [StatusEnvioCertificado] = 'EnvioIncerto';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    CREATE INDEX [IX_Participacao_StatusEnvioCertificado_ProximaTentativaCertificadoUtc] ON [Participacao] ([StatusEnvioCertificado], [ProximaTentativaCertificadoUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260910163741_AddCertificadoPdfDelivery'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260910163741_AddCertificadoPdfDelivery', N'10.0.5');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912153736_AddStatusInscricao'
)
BEGIN
    ALTER TABLE [Participacao] ADD [DataCancelamento] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912153736_AddStatusInscricao'
)
BEGIN
    ALTER TABLE [Participacao] ADD [StatusInscricao] nvarchar(20) NOT NULL DEFAULT N'Ativa';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260912153736_AddStatusInscricao'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260912153736_AddStatusInscricao', N'10.0.5');
END;

COMMIT;
GO
