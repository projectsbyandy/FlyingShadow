-- =============================================================
-- FlyingShadow Integration Test Data Setup Script
-- =============================================================

CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- =============================================================
-- TABLE: shadows
-- =============================================================
CREATE TABLE IF NOT EXISTS public.shadows (
    id         UUID DEFAULT gen_random_uuid() NOT NULL,
    code_name  TEXT NOT NULL,
    clan       TEXT NOT NULL,
    origin     TEXT NOT NULL,
    rank       TEXT NOT NULL,
    CONSTRAINT shadows_pkey PRIMARY KEY (id)
    );

-- =============================================================
-- TABLE: stealthmetrics
-- =============================================================
CREATE TABLE IF NOT EXISTS public.stealthmetrics (
    id                       UUID    DEFAULT gen_random_uuid() NOT NULL,
    shadow_id                UUID    NOT NULL,
    shadow_blend_score       INTEGER NOT NULL,
    silence_rating           INTEGER NOT NULL,
    invisibility_duration_ms INTEGER NOT NULL,
    acrobatics_level         TEXT    NOT NULL,
    CONSTRAINT stealthmetrics_pkey PRIMARY KEY (id),
    CONSTRAINT stealthmetrics_shadow_id_fkey FOREIGN KEY (shadow_id)
    REFERENCES public.shadows(id)
    );