-- =====================================================
-- Database: saasdb
-- Purpose : Initial setup for multi-tenant SaaS backend
-- =====================================================

-- 1️⃣ Create database (run only once)
CREATE DATABASE saasdb;

-- -----------------------------------------------------
-- 2️⃣ Connect to database
-- (Skip this line if your tool auto-connects)
-- -----------------------------------------------------
\c saasdb;

-- -----------------------------------------------------
-- 3️⃣ Create TENANTS table
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS tenants (
    tenant_id  UUID PRIMARY KEY,
    name       VARCHAR(200) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- -----------------------------------------------------
-- 4️⃣ Create USERS table
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    user_id    UUID PRIMARY KEY,
    email      VARCHAR(255) NOT NULL,
    tenant_id  UUID NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),

    CONSTRAINT fk_users_tenant
        FOREIGN KEY (tenant_id)
        REFERENCES tenants (tenant_id)
        ON DELETE CASCADE
);

-- -----------------------------------------------------
-- 5️⃣ Index for fast tenant-based queries
-- -----------------------------------------------------
CREATE INDEX IF NOT EXISTS idx_users_tenant_id
ON users (tenant_id);

-- -----------------------------------------------------
-- 6️⃣ Insert sample data (optional)
-- -----------------------------------------------------
INSERT INTO tenants (tenant_id, name)
VALUES ('11111111-1111-1111-1111-111111111111', 'Acme Corp')
ON CONFLICT DO NOTHING;

INSERT INTO users (user_id, email, tenant_id)
VALUES (
    '22222222-2222-2222-2222-222222222222',
    'admin@acme.com',
    '11111111-1111-1111-1111-111111111111'
)
ON CONFLICT DO NOTHING;

-- -----------------------------------------------------
-- 7️⃣ Verify data
-- -----------------------------------------------------
SELECT * FROM tenants;
SELECT * FROM users;
