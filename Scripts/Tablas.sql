-- Crear tabla Proyecto
CREATE TABLE Proyecto (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    NumeroArchivos INT NOT NULL,
    NumeroClases INT NOT NULL,
    LineasCodigo INT NOT NULL,
    NumeroMetodos INT NOT NULL
);

-- Crear tabla Clase
CREATE TABLE Clase (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    NumeroMetodos INT NOT NULL,
    NumeroPropiedades INT NOT NULL,
    LineasCodigo INT NOT NULL,
    VariablesLocales INT NOT NULL,
    PorcentajeComentarios DECIMAL(5,2) NOT NULL, -- Por ejemplo: 23.45 (%)
    SumaAscii INT NOT NULL,
    ProyectoId INT NOT NULL,
    FOREIGN KEY (ProyectoId) REFERENCES Proyecto(Id)
);

-- Crear tabla Metodo
CREATE TABLE Metodo (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    LineasCodigo INT NOT NULL,
    NumeroCiclos INT NOT NULL,
    NumeroCondicionales INT NOT NULL,
    CodigoFuente NVARCHAR(MAX) NOT NULL,
    ComplejidadCiclomatica INT NOT NULL,
    NumeroParametros INT NOT NULL,
    ClaseId INT NOT NULL,
    FOREIGN KEY (ClaseId) REFERENCES Clase(Id)
);
