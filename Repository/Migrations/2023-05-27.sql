CREATE TABLE Matricies
(
    "id" serial,
    "rows" integer,
    "columns" integer,
    "hash" text,
    PRIMARY KEY ("id")
);

CREATE TABLE Values
(
    "id" serial,
    "row" integer,
    "column" integer,
    "value" double precision NOT NULL,
    PRIMARY KEY ("id", "row", "column"),
	CONSTRAINT "IdPrimaryKey" FOREIGN KEY ("id")
        REFERENCES Matricies ("id") MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
);

CREATE INDEX MatriciesIdIndex
    ON Matricies USING btree
    ("id");
	
CREATE INDEX MatriciesHashIndex
    ON Matricies USING btree
    ("hash");

CREATE INDEX ValuesIndex
    ON Values USING btree
    ("id", "row", "column");