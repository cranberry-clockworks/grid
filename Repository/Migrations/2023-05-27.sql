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

CREATE OR REPLACE FUNCTION CheckRange()
  RETURNS TRIGGER AS $$
BEGIN
  IF EXISTS (
    SELECT 1
    FROM Matricies
    WHERE NEW.id = id AND NEW.row >= 0 AND NEW.row < rows AND NEW.column >= 0 AND NEW.column < columns
  ) THEN
    RETURN NEW;
  ELSE
    RAISE EXCEPTION 'The inserted index is not within the valid range.';
  END IF;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER ValidateRange
  BEFORE INSERT ON Values
  FOR EACH ROW
  EXECUTE FUNCTION CheckRange();

CREATE INDEX MatriciesIdIndex
    ON Matricies USING btree
    ("id");
	
CREATE INDEX MatriciesHashIndex
    ON Matricies USING btree
    ("hash");

CREATE INDEX ValuesIndex
    ON Values USING btree
    ("id", "row", "column");