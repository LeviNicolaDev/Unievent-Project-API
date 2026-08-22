-- DROP SCHEMA dbo;

CREATE SCHEMA dbo;
-- Unievent.dbo.Instituicao definition

-- Drop table

-- DROP TABLE Unievent.dbo.Instituicao;

CREATE TABLE Unievent.dbo.Instituicao (
	Id int IDENTITY(1,1) NOT NULL,
	FotoPerfil nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Cnpj nvarchar(18) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	AtualizadoEmUtc datetime2 DEFAULT getutcdate() NOT NULL,
	Codigo nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CriadoEmUtc datetime2 DEFAULT getutcdate() NOT NULL,
	IsAtivo bit DEFAULT CONVERT([bit],(1)) NOT NULL,
	Nome nvarchar(160) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	NomeAbreviado nvarchar(40) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Site nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Telefone nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	Bairro nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	Cep nvarchar(8) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	Cidade nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	Estado nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	Numero nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	Rua nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	CONSTRAINT PK_Instituicao PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Instituicao_Cnpj ON Unievent.dbo.Instituicao (  Cnpj ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX IX_Instituicao_Codigo ON Unievent.dbo.Instituicao (  Codigo ASC  )  
	 WHERE  ([Codigo] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.UsuarioUnievent definition

-- Drop table

-- DROP TABLE Unievent.dbo.UsuarioUnievent;

CREATE TABLE Unievent.dbo.UsuarioUnievent (
	Id int IDENTITY(1,1) NOT NULL,
	NomeUsuario nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	EmailUsuario nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Senha nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Chave nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RoleUsuario nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	TentativasLogin int NOT NULL,
	IsAtivo bit NOT NULL,
	CriadoEmUtc datetime2 NOT NULL,
	CONSTRAINT PK_UsuarioUnievent PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_UsuarioUnievent_Chave ON Unievent.dbo.UsuarioUnievent (  Chave ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX IX_UsuarioUnievent_EmailUsuario ON Unievent.dbo.UsuarioUnievent (  EmailUsuario ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE Unievent.dbo.[__EFMigrationsHistory];

CREATE TABLE Unievent.dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);


-- Unievent.dbo.Aluno definition

-- Drop table

-- DROP TABLE Unievent.dbo.Aluno;

CREATE TABLE Unievent.dbo.Aluno (
	Id int IDENTITY(1,1) NOT NULL,
	Nome nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Senha nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FotoPerfil nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	IsAtivo bit NOT NULL,
	DataNascimento datetime2 NOT NULL,
	[Role] int DEFAULT 0 NOT NULL,
	InstituicaoId int NULL,
	TipoParticipante nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'Interno' NOT NULL,
	ChaveConfirmacaoEmail nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	EmailConfirmado bit DEFAULT CONVERT([bit],(1)) NOT NULL,
	CONSTRAINT PK_Aluno PRIMARY KEY (Id),
	CONSTRAINT FK_Aluno_Instituicao_InstituicaoId FOREIGN KEY (InstituicaoId) REFERENCES Unievent.dbo.Instituicao(Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Aluno_ChaveConfirmacaoEmail ON Unievent.dbo.Aluno (  ChaveConfirmacaoEmail ASC  )  
	 WHERE  ([ChaveConfirmacaoEmail] IS NOT NULL)
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX IX_Aluno_Email ON Unievent.dbo.Aluno (  Email ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Aluno_InstituicaoId ON Unievent.dbo.Aluno (  InstituicaoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.PreferenciaNotificacao definition

-- Drop table

-- DROP TABLE Unievent.dbo.PreferenciaNotificacao;

CREATE TABLE Unievent.dbo.PreferenciaNotificacao (
	Id int IDENTITY(1,1) NOT NULL,
	AlunoId int NOT NULL,
	LembretesEventos bit NOT NULL,
	AlertasCertificados bit NOT NULL,
	Recomendacoes bit NOT NULL,
	UsarLocalizacao bit NOT NULL,
	Categorias nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	LatitudeAproximada float NULL,
	LongitudeAproximada float NULL,
	RaioKm int NOT NULL,
	AtualizadoEmUtc datetime2 NOT NULL,
	CONSTRAINT PK_PreferenciaNotificacao PRIMARY KEY (Id),
	CONSTRAINT FK_PreferenciaNotificacao_Aluno_AlunoId FOREIGN KEY (AlunoId) REFERENCES Unievent.dbo.Aluno(Id) ON DELETE CASCADE
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_PreferenciaNotificacao_AlunoId ON Unievent.dbo.PreferenciaNotificacao (  AlunoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.ResponsavelEvento definition

-- Drop table

-- DROP TABLE Unievent.dbo.ResponsavelEvento;

CREATE TABLE Unievent.dbo.ResponsavelEvento (
	Id int IDENTITY(1,1) NOT NULL,
	Nome nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FotoPerfil nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	InstituicaoId int NULL,
	CONSTRAINT PK_ResponsavelEvento PRIMARY KEY (Id),
	CONSTRAINT FK_ResponsavelEvento_Instituicao_InstituicaoId FOREIGN KEY (InstituicaoId) REFERENCES Unievent.dbo.Instituicao(Id)
);
 CREATE NONCLUSTERED INDEX IX_ResponsavelEvento_InstituicaoId ON Unievent.dbo.ResponsavelEvento (  InstituicaoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.UsuarioSecretaria definition

-- Drop table

-- DROP TABLE Unievent.dbo.UsuarioSecretaria;

CREATE TABLE Unievent.dbo.UsuarioSecretaria (
	Id int IDENTITY(1,1) NOT NULL,
	NomeUsuario nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RoleUsuario nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	EmailUsuario nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Chave nvarchar(300) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	TentativasLogin int NOT NULL,
	IsAtivo bit NOT NULL,
	Senha nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'' NOT NULL,
	InstituicaoId int NULL,
	Status nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'Ativo' NOT NULL,
	EmailConfirmado bit DEFAULT CONVERT([bit],(1)) NOT NULL,
	CONSTRAINT PK_UsuarioSecretaria PRIMARY KEY (Id),
	CONSTRAINT FK_UsuarioSecretaria_Instituicao_InstituicaoId FOREIGN KEY (InstituicaoId) REFERENCES Unievent.dbo.Instituicao(Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_UsuarioSecretaria_Chave ON Unievent.dbo.UsuarioSecretaria (  Chave ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX IX_UsuarioSecretaria_EmailUsuario ON Unievent.dbo.UsuarioSecretaria (  EmailUsuario ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_UsuarioSecretaria_InstituicaoId ON Unievent.dbo.UsuarioSecretaria (  InstituicaoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.Evento definition

-- Drop table

-- DROP TABLE Unievent.dbo.Evento;

CREATE TABLE Unievent.dbo.Evento (
	Id int IDENTITY(1,1) NOT NULL,
	Nome nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Descricao nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Categoria nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	DataEvento datetime2 NOT NULL,
	ResponsavelEventoId int NOT NULL,
	Capacidade int NOT NULL,
	Thumbnail nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	FimInscricoes datetime2 NULL,
	InicioInscricoes datetime2 NULL,
	InstituicaoId int NULL,
	Latitude float NULL,
	Longitude float NULL,
	PublicoPermitido nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	RaioCheckInMetros int DEFAULT 150 NOT NULL,
	ToleranciaCheckInMinutos int DEFAULT 60 NOT NULL,
	Visibilidade nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Local] nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_Evento PRIMARY KEY (Id),
	CONSTRAINT FK_Evento_Instituicao_InstituicaoId FOREIGN KEY (InstituicaoId) REFERENCES Unievent.dbo.Instituicao(Id),
	CONSTRAINT FK_Evento_ResponsavelEvento_ResponsavelEventoId FOREIGN KEY (ResponsavelEventoId) REFERENCES Unievent.dbo.ResponsavelEvento(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Evento_InstituicaoId_Categoria ON Unievent.dbo.Evento (  InstituicaoId ASC  , Categoria ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Evento_InstituicaoId_DataEvento ON Unievent.dbo.Evento (  InstituicaoId ASC  , DataEvento ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Evento_ResponsavelEventoId ON Unievent.dbo.Evento (  ResponsavelEventoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Evento_Visibilidade_PublicoPermitido ON Unievent.dbo.Evento (  Visibilidade ASC  , PublicoPermitido ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.NotificacaoEvento definition

-- Drop table

-- DROP TABLE Unievent.dbo.NotificacaoEvento;

CREATE TABLE Unievent.dbo.NotificacaoEvento (
	Id int IDENTITY(1,1) NOT NULL,
	AlunoId int NOT NULL,
	EventoId int NOT NULL,
	Tipo nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Assunto nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Mensagem nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProcessadaEmUtc datetime2 NOT NULL,
	Enviada bit NOT NULL,
	Erro nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_NotificacaoEvento PRIMARY KEY (Id),
	CONSTRAINT FK_NotificacaoEvento_Aluno_AlunoId FOREIGN KEY (AlunoId) REFERENCES Unievent.dbo.Aluno(Id) ON DELETE CASCADE,
	CONSTRAINT FK_NotificacaoEvento_Evento_EventoId FOREIGN KEY (EventoId) REFERENCES Unievent.dbo.Evento(Id) ON DELETE CASCADE
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_NotificacaoEvento_AlunoId_EventoId_Tipo ON Unievent.dbo.NotificacaoEvento (  AlunoId ASC  , EventoId ASC  , Tipo ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_NotificacaoEvento_EventoId ON Unievent.dbo.NotificacaoEvento (  EventoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.Participacao definition

-- Drop table

-- DROP TABLE Unievent.dbo.Participacao;

CREATE TABLE Unievent.dbo.Participacao (
	Id int IDENTITY(1,1) NOT NULL,
	AlunoId int NOT NULL,
	EventoId int NOT NULL,
	PresencaConfirmada bit NOT NULL,
	DataConfirmacao datetime2 NULL,
	CertificadoEmitido bit NOT NULL,
	CodigoValidacao nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CodigoIngresso nvarchar(450) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT lower(replace(CONVERT([varchar](36),newid()),'-','')) NOT NULL,
	DistanciaCheckInMetros float NULL,
	OperadorCheckInId int NULL,
	PrecisaoLocalizacaoMetros float NULL,
	CertificadoEnviadoPorEmail bit DEFAULT CONVERT([bit],(0)) NOT NULL,
	DataEnvioCertificadoEmail datetime2 NULL,
	ErroEnvioCertificadoEmail nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	CONSTRAINT PK_Participacao PRIMARY KEY (Id),
	CONSTRAINT FK_Participacao_Aluno_AlunoId FOREIGN KEY (AlunoId) REFERENCES Unievent.dbo.Aluno(Id) ON DELETE CASCADE,
	CONSTRAINT FK_Participacao_Evento_EventoId FOREIGN KEY (EventoId) REFERENCES Unievent.dbo.Evento(Id) ON DELETE CASCADE
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Participacao_AlunoId_EventoId ON Unievent.dbo.Participacao (  AlunoId ASC  , EventoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE UNIQUE NONCLUSTERED INDEX IX_Participacao_CodigoIngresso ON Unievent.dbo.Participacao (  CodigoIngresso ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Participacao_EventoId ON Unievent.dbo.Participacao (  EventoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- Unievent.dbo.Certificado definition

-- Drop table

-- DROP TABLE Unievent.dbo.Certificado;

CREATE TABLE Unievent.dbo.Certificado (
	Id int IDENTITY(1,1) NOT NULL,
	DataCertifcado datetime2 NOT NULL,
	Texto nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	EventoId int NOT NULL,
	CONSTRAINT PK_Certificado PRIMARY KEY (Id),
	CONSTRAINT FK_Certificado_Evento_EventoId FOREIGN KEY (EventoId) REFERENCES Unievent.dbo.Evento(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Certificado_EventoId ON Unievent.dbo.Certificado (  EventoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


