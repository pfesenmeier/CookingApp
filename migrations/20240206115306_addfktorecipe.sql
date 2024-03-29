BEGIN;
  ALTER TABLE recipes ADD IF NOT EXISTS "userid" uuid;
  ALTER TABLE users ALTER COLUMN id SET DEFAULT gen_random_uuid();
  ALTER TABLE users ADD CONSTRAINT username_unique UNIQUE (username);
  ALTER TABLE recipes ADD CONSTRAINT fk_user FOREIGN KEY (userid) REFERENCES users (id);
COMMIT;
