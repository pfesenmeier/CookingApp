BEGIN;
  ALTER TABLE recipes ADD COLUMN IF NOT EXISTS "id" uuid;
  SELECT * FROM recipes FOR UPDATE;
  UPDATE recipes SET "id" = gen_random_uuid() WHERE "id" IS NULL;
COMMIT
