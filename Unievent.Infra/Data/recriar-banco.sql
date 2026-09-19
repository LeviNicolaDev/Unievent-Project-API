/*
    UniEvent - reset completo do banco de desenvolvimento

    ATENÇÃO: este script apaga permanentemente o banco [Unievent],
    incluindo todos os usuários, eventos, ingressos e certificados.

    Compatível com clientes que executam T-SQL comum, como DBeaver,
    Azure Data Studio e SQL Server Management Studio.

    Depois de executar, inicie ou reinicie a API em ambiente Development.
    A opção Database:MigrateOnStartup aplicará todas as migrations.
*/

USE [master];

IF DB_ID(N'Unievent') IS NOT NULL
BEGIN
    PRINT N'Encerrando conexões ativas com o banco [Unievent]...';
    ALTER DATABASE [Unievent] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    PRINT N'Removendo o banco [Unievent]...';
    DROP DATABASE [Unievent];
END;

PRINT N'Criando o banco [Unievent]...';
EXEC(N'CREATE DATABASE [Unievent]');

ALTER DATABASE [Unievent] SET RECOVERY SIMPLE;
ALTER DATABASE [Unievent] SET MULTI_USER;

PRINT N'Banco [Unievent] recriado com sucesso e pronto para receber as migrations.';
PRINT N'Inicie ou reinicie a API para criar o esquema automaticamente.';
