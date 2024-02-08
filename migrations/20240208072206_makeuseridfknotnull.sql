BEGIN;
  SELECT * FROM recipes FOR UPDATE;
  -- https://www.postgresql.org/docs/current/ddl-alter.html#DDL-ALTER-ADDING-A-CONSTRAINT
  UPDATE recipes SET "userid" = (SELECT id FROM users WHERE username = 'paul');
  ALTER TABLE recipes ALTER COLUMN userid SET NOT NULL;
COMMIT;
