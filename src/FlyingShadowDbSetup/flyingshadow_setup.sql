-- =============================================================
-- FlyingShadow Database Setup Script
-- Usage: psql -U <admin / superuser> -d postgres -f flyingshadow_setup.sql
-- =============================================================

-- Create the database (run this as a superuser / postgres role)
CREATE DATABASE flyingshadow
    WITH TEMPLATE = template0
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8';

-- Connect to the new database before running the rest
\connect flyingshadow

-- gen_random_uuid() is built-in from PostgreSQL 13+.
-- For PostgreSQL 10-12, pgcrypto provides it. This is a no-op on 13+.
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- =============================================================
-- TABLE: shadows
-- =============================================================
CREATE TABLE public.shadows (
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
CREATE TABLE public.stealthmetrics (
    id                       UUID    DEFAULT gen_random_uuid() NOT NULL,
    shadow_id                UUID    NOT NULL,
    shadow_blend_score       INTEGER NOT NULL,
    silence_rating           INTEGER NOT NULL,
    invisibility_duration_ms INTEGER NOT NULL,
    acrobatics_level         TEXT    NOT NULL,
    CONSTRAINT stealthmetrics_pkey PRIMARY KEY (id)
);

-- =============================================================
-- TABLE: users
-- =============================================================
CREATE TABLE public.users (
    user_id         UUID DEFAULT gen_random_uuid() NOT NULL,
    email           TEXT NOT NULL,
    hashed_password TEXT NOT NULL,
    CONSTRAINT users_pkey PRIMARY KEY (user_id)
);

-- =============================================================
-- TABLE: testsupport
-- =============================================================
CREATE TABLE public.testsupport (
    jwt TEXT NOT NULL
);

-- =============================================================
-- DATA: shadows  (150 rows)
-- =============================================================
INSERT INTO public.shadows (id, code_name, clan, origin, rank) VALUES
('550e8400-e29b-41d4-a716-000000000031', 'Shadow Wolf II',    'Hidden Rain',       'Land of Lightning', 'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000032', 'Shadow Fox',        'Hidden Waterfall',  'Land of Water',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000033', 'Shadow Tiger',      'Hidden Cloud',      'Land of Earth',     'Danza'),
('550e8400-e29b-41d4-a716-000000000034', 'Shadow Dragon',     'Hidden Sound',      'Land of Sand',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000035', 'Shadow Hawk III',   'Akatsuki',          'Land of Wind',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000036', 'Shadow Viper',      'Seven Swordsmen',   'Mist Country',      'Danza'),
('550e8400-e29b-41d4-a716-000000000037', 'Shadow Panther',    'Anbu',              'Various',           'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000038', 'Shadow Bear II',    'Hyuga Clan',        'Land of Rain',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000039', 'Shadow Owl',        'Uzumaki Clan',      'Land of Sound',     'Danza'),
('550e8400-e29b-41d4-a716-000000000040', 'Shadow Cobra',      'Uchiha Clan',       'Land of Fire',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000041', 'Shadow Raven',      'Sannin',            'Land of Lightning', 'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000042', 'Shadow Spider',     'Kumo Clan',         'Land of Water',     'Danza'),
('550e8400-e29b-41d4-a716-000000000043', 'Shadow Lynx',       'Ronin',             'Land of Earth',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000044', 'Shadow Crane',      'Kirigakure',        'Land of Sand',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000045', 'Shadow Mantis',     'Iwagakure',         'Land of Wind',      'Danza'),
('550e8400-e29b-41d4-a716-000000000046', 'Shadow Scorpion',   'Sunagakure',        'Mist Country',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000047', 'Shadow Lotus',      'Hidden Leaf',       'Various',           'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000048', 'Shadow Blade II',   'Hidden Mist',       'Land of Rain',      'Danza'),
('550e8400-e29b-41d4-a716-000000000049', 'Shadow Fang',       'Hidden Stone',      'Land of Sound',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000050', 'Shadow Claw',       'Hidden Sand',       'Land of Fire',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000051', 'Shadow Mirage',     'Hidden Rain',       'Land of Lightning', 'Danza'),
('550e8400-e29b-41d4-a716-000000000052', 'Shadow Specter',    'Hidden Waterfall',  'Land of Water',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000053', 'Shadow Wraith',     'Hidden Cloud',      'Land of Earth',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000054', 'Shadow Shade',      'Hidden Sound',      'Land of Sand',      'Danza'),
('550e8400-e29b-41d4-a716-000000000055', 'Shadow Tempest',    'Akatsuki',          'Land of Wind',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000056', 'Shadow Wisp',       'Seven Swordsmen',   'Mist Country',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000057', 'Shadow Shroud',     'Anbu',              'Various',           'Danza'),
('550e8400-e29b-41d4-a716-000000000058', 'Shadow Dagger',     'Hyuga Clan',        'Land of Rain',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000059', 'Shadow Talon',      'Uzumaki Clan',      'Land of Sound',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000060', 'Shadow Serpent',    'Uchiha Clan',       'Land of Fire',      'Danza'),
('550e8400-e29b-41d4-a716-000000000061', 'Shadow Phoenix',    'Sannin',            'Land of Lightning', 'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000062', 'Shadow Hydra',      'Kumo Clan',         'Land of Water',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000063', 'Shadow Basilisk',   'Ronin',             'Land of Earth',     'Danza'),
('550e8400-e29b-41d4-a716-000000000064', 'Shadow Wyvern',     'Kirigakure',        'Land of Sand',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000065', 'Shadow Banshee',    'Iwagakure',         'Land of Wind',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000066', 'Shadow Revenant',   'Sunagakure',        'Mist Country',      'Danza'),
('550e8400-e29b-41d4-a716-000000000067', 'Shadow Phantom',    'Hidden Leaf',       'Various',           'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000068', 'Shadow Samurai',    'Hidden Mist',       'Land of Rain',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000069', 'Shadow Ronin',      'Hidden Stone',      'Land of Sound',     'Danza'),
('550e8400-e29b-41d4-a716-000000000070', 'Shadow Monk',       'Hidden Sand',       'Land of Fire',      'Toshiyama'),
-- Dark series
('550e8400-e29b-41d4-a716-000000000071', 'Dark Wolf',         'Hidden Rain',       'Land of Lightning', 'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000072', 'Dark Fox',          'Hidden Waterfall',  'Land of Water',     'Danza'),
('550e8400-e29b-41d4-a716-000000000073', 'Dark Tiger',        'Hidden Cloud',      'Land of Earth',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000074', 'Dark Dragon',       'Hidden Sound',      'Land of Sand',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000075', 'Dark Hawk',         'Akatsuki',          'Land of Wind',      'Danza'),
('550e8400-e29b-41d4-a716-000000000076', 'Dark Viper',        'Seven Swordsmen',   'Mist Country',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000077', 'Dark Panther II',   'Anbu',              'Various',           'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000078', 'Dark Bear',         'Hyuga Clan',        'Land of Rain',      'Danza'),
('550e8400-e29b-41d4-a716-000000000079', 'Dark Owl',          'Uzumaki Clan',      'Land of Sound',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000080', 'Dark Cobra',        'Uchiha Clan',       'Land of Fire',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000081', 'Dark Raven',        'Sannin',            'Land of Lightning', 'Danza'),
('550e8400-e29b-41d4-a716-000000000082', 'Dark Spider II',    'Kumo Clan',         'Land of Water',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000083', 'Dark Lynx',         'Ronin',             'Land of Earth',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000084', 'Dark Crane',        'Kirigakure',        'Land of Sand',      'Danza'),
('550e8400-e29b-41d4-a716-000000000085', 'Dark Mantis',       'Iwagakure',         'Land of Wind',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000086', 'Dark Scorpion',     'Sunagakure',        'Mist Country',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000087', 'Dark Lotus',        'Hidden Leaf',       'Various',           'Danza'),
('550e8400-e29b-41d4-a716-000000000088', 'Dark Blade',        'Hidden Mist',       'Land of Rain',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000089', 'Dark Fang',         'Hidden Stone',      'Land of Sound',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000090', 'Dark Claw',         'Hidden Sand',       'Land of Fire',      'Danza'),
('550e8400-e29b-41d4-a716-000000000091', 'Dark Mirage II',    'Hidden Rain',       'Land of Lightning', 'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000092', 'Dark Specter',      'Hidden Waterfall',  'Land of Water',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000093', 'Dark Wraith',       'Hidden Cloud',      'Land of Earth',     'Danza'),
('550e8400-e29b-41d4-a716-000000000094', 'Dark Shade',        'Hidden Sound',      'Land of Sand',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000095', 'Dark Tempest',      'Akatsuki',          'Land of Wind',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000096', 'Dark Wisp',         'Seven Swordsmen',   'Mist Country',      'Danza'),
('550e8400-e29b-41d4-a716-000000000097', 'Dark Shroud',       'Anbu',              'Various',           'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000098', 'Dark Dagger',       'Hyuga Clan',        'Land of Rain',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000099', 'Dark Talon',        'Uzumaki Clan',      'Land of Sound',     'Danza'),
('550e8400-e29b-41d4-a716-000000000100', 'Dark Serpent',      'Uchiha Clan',       'Land of Fire',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000101', 'Dark Phoenix',      'Sannin',            'Land of Lightning', 'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000102', 'Dark Hydra',        'Kumo Clan',         'Land of Water',     'Danza'),
('550e8400-e29b-41d4-a716-000000000103', 'Dark Basilisk',     'Ronin',             'Land of Earth',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000104', 'Dark Wyvern',       'Kirigakure',        'Land of Sand',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000105', 'Dark Banshee',      'Iwagakure',         'Land of Wind',      'Danza'),
('550e8400-e29b-41d4-a716-000000000106', 'Dark Revenant',     'Sunagakure',        'Mist Country',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000107', 'Dark Phantom',      'Hidden Leaf',       'Various',           'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000108', 'Dark Samurai',      'Hidden Mist',       'Land of Rain',      'Danza'),
('550e8400-e29b-41d4-a716-000000000109', 'Dark Ronin',        'Hidden Stone',      'Land of Sound',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000110', 'Dark Monk',         'Hidden Sand',       'Land of Fire',      'Oniwaban'),
-- Night series
('550e8400-e29b-41d4-a716-000000000111', 'Night Wolf',        'Hidden Rain',       'Land of Lightning', 'Danza'),
('550e8400-e29b-41d4-a716-000000000112', 'Night Fox II',      'Hidden Waterfall',  'Land of Water',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000113', 'Night Tiger',       'Hidden Cloud',      'Land of Earth',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000114', 'Night Dragon',      'Hidden Sound',      'Land of Sand',      'Danza'),
('550e8400-e29b-41d4-a716-000000000115', 'Night Hawk',        'Akatsuki',          'Land of Wind',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000116', 'Night Viper',       'Seven Swordsmen',   'Mist Country',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000117', 'Night Panther II',  'Anbu',              'Various',           'Danza'),
('550e8400-e29b-41d4-a716-000000000118', 'Night Bear',        'Hyuga Clan',        'Land of Rain',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000119', 'Night Owl II',      'Uzumaki Clan',      'Land of Sound',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000120', 'Night Cobra',       'Uchiha Clan',       'Land of Fire',      'Danza'),
('550e8400-e29b-41d4-a716-000000000121', 'Night Raven II',    'Sannin',            'Land of Lightning', 'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000122', 'Night Spider',      'Kumo Clan',         'Land of Water',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000123', 'Night Lynx',        'Ronin',             'Land of Earth',     'Danza'),
('550e8400-e29b-41d4-a716-000000000124', 'Night Crane',       'Kirigakure',        'Land of Sand',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000125', 'Night Mantis',      'Iwagakure',         'Land of Wind',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000126', 'Night Scorpion',    'Sunagakure',        'Mist Country',      'Danza'),
('550e8400-e29b-41d4-a716-000000000127', 'Night Lotus',       'Hidden Leaf',       'Various',           'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000128', 'Night Blade',       'Hidden Mist',       'Land of Rain',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000129', 'Night Fang',        'Hidden Stone',      'Land of Sound',     'Danza'),
('550e8400-e29b-41d4-a716-000000000130', 'Night Claw',        'Hidden Sand',       'Land of Fire',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000131', 'Night Mirage',      'Hidden Rain',       'Land of Lightning', 'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000132', 'Night Specter',     'Hidden Waterfall',  'Land of Water',     'Danza'),
('550e8400-e29b-41d4-a716-000000000133', 'Night Wraith',      'Hidden Cloud',      'Land of Earth',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000134', 'Night Shade',       'Hidden Sound',      'Land of Sand',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000135', 'Night Tempest',     'Akatsuki',          'Land of Wind',      'Danza'),
('550e8400-e29b-41d4-a716-000000000136', 'Night Wisp',        'Seven Swordsmen',   'Mist Country',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000137', 'Night Shroud',      'Anbu',              'Various',           'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000138', 'Night Dagger',      'Hyuga Clan',        'Land of Rain',      'Danza'),
('550e8400-e29b-41d4-a716-000000000139', 'Night Talon',       'Uzumaki Clan',      'Land of Sound',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000140', 'Night Serpent',     'Uchiha Clan',       'Land of Fire',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000141', 'Night Phoenix',     'Sannin',            'Land of Lightning', 'Danza'),
('550e8400-e29b-41d4-a716-000000000142', 'Night Hydra',       'Kumo Clan',         'Land of Water',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000143', 'Night Basilisk',    'Ronin',             'Land of Earth',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000144', 'Night Wyvern',      'Kirigakure',        'Land of Sand',      'Danza'),
('550e8400-e29b-41d4-a716-000000000145', 'Night Banshee',     'Iwagakure',         'Land of Wind',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000146', 'Night Revenant',    'Sunagakure',        'Mist Country',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000147', 'Night Phantom',     'Hidden Leaf',       'Various',           'Danza'),
('550e8400-e29b-41d4-a716-000000000148', 'Night Samurai',     'Hidden Mist',       'Land of Rain',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000149', 'Night Ronin',       'Hidden Stone',      'Land of Sound',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000150', 'Night Monk',        'Hidden Sand',       'Land of Fire',      'Danza'),
-- Silent series
('550e8400-e29b-41d4-a716-000000000151', 'Silent Wolf II',    'Hidden Rain',       'Land of Lightning', 'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000152', 'Silent Fox',        'Hidden Waterfall',  'Land of Water',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000153', 'Silent Tiger II',   'Hidden Cloud',      'Land of Earth',     'Danza'),
('550e8400-e29b-41d4-a716-000000000154', 'Silent Dragon',     'Hidden Sound',      'Land of Sand',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000155', 'Silent Hawk',       'Akatsuki',          'Land of Wind',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000156', 'Silent Viper',      'Seven Swordsmen',   'Mist Country',      'Danza'),
('550e8400-e29b-41d4-a716-000000000157', 'Silent Panther',    'Anbu',              'Various',           'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000158', 'Silent Bear',       'Hyuga Clan',        'Land of Rain',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000159', 'Silent Owl',        'Uzumaki Clan',      'Land of Sound',     'Danza'),
('550e8400-e29b-41d4-a716-000000000160', 'Silent Cobra',      'Uchiha Clan',       'Land of Fire',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000161', 'Silent Raven',      'Sannin',            'Land of Lightning', 'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000162', 'Silent Spider',     'Kumo Clan',         'Land of Water',     'Danza'),
('550e8400-e29b-41d4-a716-000000000163', 'Silent Lynx',       'Ronin',             'Land of Earth',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000164', 'Silent Crane',      'Kirigakure',        'Land of Sand',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000165', 'Silent Mantis',     'Iwagakure',         'Land of Wind',      'Danza'),
('550e8400-e29b-41d4-a716-000000000166', 'Silent Scorpion',   'Sunagakure',        'Mist Country',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000167', 'Silent Lotus',      'Hidden Leaf',       'Various',           'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000168', 'Silent Blade',      'Hidden Mist',       'Land of Rain',      'Danza'),
('550e8400-e29b-41d4-a716-000000000169', 'Silent Fang',       'Hidden Stone',      'Land of Sound',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000170', 'Silent Claw',       'Hidden Sand',       'Land of Fire',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000171', 'Silent Mirage',     'Hidden Rain',       'Land of Lightning', 'Danza'),
('550e8400-e29b-41d4-a716-000000000172', 'Silent Specter',    'Hidden Waterfall',  'Land of Water',     'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000173', 'Silent Wraith',     'Hidden Cloud',      'Land of Earth',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000174', 'Silent Shade',      'Hidden Sound',      'Land of Sand',      'Danza'),
('550e8400-e29b-41d4-a716-000000000175', 'Silent Tempest',    'Akatsuki',          'Land of Wind',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000176', 'Silent Wisp',       'Seven Swordsmen',   'Mist Country',      'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000177', 'Silent Shroud',     'Anbu',              'Various',           'Danza'),
('550e8400-e29b-41d4-a716-000000000178', 'Silent Dagger',     'Hyuga Clan',        'Land of Rain',      'Toshiyama'),
('550e8400-e29b-41d4-a716-000000000179', 'Silent Talon',      'Uzumaki Clan',      'Land of Sound',     'Oniwaban'),
('550e8400-e29b-41d4-a716-000000000180', 'Silent Serpent',    'Uchiha Clan',       'Land of Fire',      'Danza');

-- =============================================================
-- DATA: stealthmetrics  (150 rows)
-- =============================================================
INSERT INTO public.stealthmetrics (id, shadow_id, shadow_blend_score, silence_rating, invisibility_duration_ms, acrobatics_level) VALUES
('b2c3d4e5-f6a7-4b8c-9d0e-000000000031', '550e8400-e29b-41d4-a716-000000000031',  53,  28, 3667, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000032', '550e8400-e29b-41d4-a716-000000000032',  12,   7, 4936, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000033', '550e8400-e29b-41d4-a716-000000000033',  52,  55, 4851, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000034', '550e8400-e29b-41d4-a716-000000000034',  41,  75, 2996, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000035', '550e8400-e29b-41d4-a716-000000000035',  55,  48, 1022, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000036', '550e8400-e29b-41d4-a716-000000000036',  11,  49, 3714, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000037', '550e8400-e29b-41d4-a716-000000000037',  25,  65, 1770, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000038', '550e8400-e29b-41d4-a716-000000000038',  56, 100, 4188, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000039', '550e8400-e29b-41d4-a716-000000000039',  87,  52, 3199, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000040', '550e8400-e29b-41d4-a716-000000000040', 100,  11, 1144, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000041', '550e8400-e29b-41d4-a716-000000000041',  83,  66, 3479, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000042', '550e8400-e29b-41d4-a716-000000000042',  34,  55, 4049, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000043', '550e8400-e29b-41d4-a716-000000000043',  12,  22, 2366, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000044', '550e8400-e29b-41d4-a716-000000000044',  74,  91, 4277, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000045', '550e8400-e29b-41d4-a716-000000000045',  33,  11, 1393, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000046', '550e8400-e29b-41d4-a716-000000000046',  45,  31, 1002, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000047', '550e8400-e29b-41d4-a716-000000000047',  69,  95, 4735, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000048', '550e8400-e29b-41d4-a716-000000000048',  84,  22, 4603, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000049', '550e8400-e29b-41d4-a716-000000000049',  11,  71, 2876, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000050', '550e8400-e29b-41d4-a716-000000000050',   1,  28, 3957, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000051', '550e8400-e29b-41d4-a716-000000000051',  89,  14, 2122, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000052', '550e8400-e29b-41d4-a716-000000000052',  52,   6, 1544, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000053', '550e8400-e29b-41d4-a716-000000000053',  91,  96, 4605, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000054', '550e8400-e29b-41d4-a716-000000000054',  15,   8, 1663, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000055', '550e8400-e29b-41d4-a716-000000000055',  31,   2, 3929, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000056', '550e8400-e29b-41d4-a716-000000000056',  13,  72, 1306, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000057', '550e8400-e29b-41d4-a716-000000000057',  38,  82, 4427, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000058', '550e8400-e29b-41d4-a716-000000000058',  22,  41, 2265, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000059', '550e8400-e29b-41d4-a716-000000000059',  93,  36, 3887, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000060', '550e8400-e29b-41d4-a716-000000000060',  72,  35, 3739, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000061', '550e8400-e29b-41d4-a716-000000000061',  34,  28, 1353, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000062', '550e8400-e29b-41d4-a716-000000000062',  51,  80, 3752, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000063', '550e8400-e29b-41d4-a716-000000000063',  79,  91, 3249, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000064', '550e8400-e29b-41d4-a716-000000000064',   8,  30, 1635, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000065', '550e8400-e29b-41d4-a716-000000000065',  40,  35, 1704, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000066', '550e8400-e29b-41d4-a716-000000000066',  89,  30, 4512, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000067', '550e8400-e29b-41d4-a716-000000000067',  74,  71, 3333, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000068', '550e8400-e29b-41d4-a716-000000000068',  73,  85, 2663, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000069', '550e8400-e29b-41d4-a716-000000000069',  16,  39, 1563, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000070', '550e8400-e29b-41d4-a716-000000000070',  62,  68, 3050, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000071', '550e8400-e29b-41d4-a716-000000000071',  19,  71, 2595, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000072', '550e8400-e29b-41d4-a716-000000000072',  14,  22, 1148, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000073', '550e8400-e29b-41d4-a716-000000000073',  72,  82, 2762, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000074', '550e8400-e29b-41d4-a716-000000000074',  37,  59, 4601, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000075', '550e8400-e29b-41d4-a716-000000000075',  77,  82, 2547, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000076', '550e8400-e29b-41d4-a716-000000000076',  48,  74, 2214, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000077', '550e8400-e29b-41d4-a716-000000000077',  19,  71, 2520, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000078', '550e8400-e29b-41d4-a716-000000000078',  66,  80, 2168, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000079', '550e8400-e29b-41d4-a716-000000000079',  70,  58, 3733, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000080', '550e8400-e29b-41d4-a716-000000000080',  54,  82, 3885, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000081', '550e8400-e29b-41d4-a716-000000000081',  35,  35, 3042, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000082', '550e8400-e29b-41d4-a716-000000000082',   1,  35, 4831, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000083', '550e8400-e29b-41d4-a716-000000000083',  51,   5, 4702, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000084', '550e8400-e29b-41d4-a716-000000000084',  28,  97, 1491, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000085', '550e8400-e29b-41d4-a716-000000000085',  83,  62, 4817, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000086', '550e8400-e29b-41d4-a716-000000000086',   9,   3, 3309, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000087', '550e8400-e29b-41d4-a716-000000000087',  56,  48, 3628, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000088', '550e8400-e29b-41d4-a716-000000000088',  58,  86, 1158, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000089', '550e8400-e29b-41d4-a716-000000000089',  46,  46, 4213, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000090', '550e8400-e29b-41d4-a716-000000000090',  31,  47, 2592, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000091', '550e8400-e29b-41d4-a716-000000000091',  66,  51, 2886, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000092', '550e8400-e29b-41d4-a716-000000000092',  98,  22, 3961, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000093', '550e8400-e29b-41d4-a716-000000000093',  37,  13, 2020, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000094', '550e8400-e29b-41d4-a716-000000000094',  13,  68, 3967, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000095', '550e8400-e29b-41d4-a716-000000000095',   7,  99, 1706, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000096', '550e8400-e29b-41d4-a716-000000000096',  99,  35, 3664, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000097', '550e8400-e29b-41d4-a716-000000000097',  77,  12, 1384, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000098', '550e8400-e29b-41d4-a716-000000000098',  51,  17, 1928, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000099', '550e8400-e29b-41d4-a716-000000000099',  37,  85,  992, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000100', '550e8400-e29b-41d4-a716-000000000100',  25,  61, 3755, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000101', '550e8400-e29b-41d4-a716-000000000101',  64,  23, 1597, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000102', '550e8400-e29b-41d4-a716-000000000102',  13,  33, 3543, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000103', '550e8400-e29b-41d4-a716-000000000103',  62,  92, 4445, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000104', '550e8400-e29b-41d4-a716-000000000104',  60,  55, 1600, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000105', '550e8400-e29b-41d4-a716-000000000105',  44,  52, 4710, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000106', '550e8400-e29b-41d4-a716-000000000106',  36,  57, 2250, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000107', '550e8400-e29b-41d4-a716-000000000107',  78,  85, 2887, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000108', '550e8400-e29b-41d4-a716-000000000108',  25,  68, 3376, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000109', '550e8400-e29b-41d4-a716-000000000109',  14,  60, 1859, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000110', '550e8400-e29b-41d4-a716-000000000110',  61,  43, 2489, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000111', '550e8400-e29b-41d4-a716-000000000111',  65,   3, 3139, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000112', '550e8400-e29b-41d4-a716-000000000112',  51,  40, 4698, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000113', '550e8400-e29b-41d4-a716-000000000113',  35,  20, 3660, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000114', '550e8400-e29b-41d4-a716-000000000114',  54,  58, 2338, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000115', '550e8400-e29b-41d4-a716-000000000115',  35,  79, 1778, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000116', '550e8400-e29b-41d4-a716-000000000116',  72,  37,  956, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000117', '550e8400-e29b-41d4-a716-000000000117',  50,  13, 2868, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000118', '550e8400-e29b-41d4-a716-000000000118',  22,  90, 3905, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000119', '550e8400-e29b-41d4-a716-000000000119',  41,   6, 2479, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000120', '550e8400-e29b-41d4-a716-000000000120',  46,  22, 1325, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000121', '550e8400-e29b-41d4-a716-000000000121',  43,  35,  984, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000122', '550e8400-e29b-41d4-a716-000000000122',  71,  74, 2019, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000123', '550e8400-e29b-41d4-a716-000000000123',  62,  15, 1961, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000124', '550e8400-e29b-41d4-a716-000000000124',   2,  58, 4003, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000125', '550e8400-e29b-41d4-a716-000000000125',  47,  48, 1816, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000126', '550e8400-e29b-41d4-a716-000000000126',  47,  29, 3671, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000127', '550e8400-e29b-41d4-a716-000000000127',   5,  90, 3473, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000128', '550e8400-e29b-41d4-a716-000000000128',  96,  80, 4554, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000129', '550e8400-e29b-41d4-a716-000000000129',  26,  74, 4175, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000130', '550e8400-e29b-41d4-a716-000000000130',  90,  94, 2974, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000131', '550e8400-e29b-41d4-a716-000000000131',  67,  18, 2976, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000132', '550e8400-e29b-41d4-a716-000000000132',  33,   5, 1136, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000133', '550e8400-e29b-41d4-a716-000000000133',  67,  60, 4439, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000134', '550e8400-e29b-41d4-a716-000000000134',  44,  57, 4433, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000135', '550e8400-e29b-41d4-a716-000000000135',  35,  79, 3219, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000136', '550e8400-e29b-41d4-a716-000000000136',  89,  12, 3689, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000137', '550e8400-e29b-41d4-a716-000000000137',  59,  83, 1207, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000138', '550e8400-e29b-41d4-a716-000000000138',  88,  73, 4562, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000139', '550e8400-e29b-41d4-a716-000000000139',  69,  20, 2066, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000140', '550e8400-e29b-41d4-a716-000000000140',  53,  16, 1748, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000141', '550e8400-e29b-41d4-a716-000000000141',  29,  49, 3238, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000142', '550e8400-e29b-41d4-a716-000000000142',   2,  38, 3963, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000143', '550e8400-e29b-41d4-a716-000000000143',  12,  66, 4961, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000144', '550e8400-e29b-41d4-a716-000000000144',  18,  49, 2291, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000145', '550e8400-e29b-41d4-a716-000000000145',  75,  48, 4487, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000146', '550e8400-e29b-41d4-a716-000000000146', 100,  73, 2133, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000147', '550e8400-e29b-41d4-a716-000000000147',  68,  78, 2466, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000148', '550e8400-e29b-41d4-a716-000000000148',  98,  26,  840, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000149', '550e8400-e29b-41d4-a716-000000000149',  41,  38, 2652, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000150', '550e8400-e29b-41d4-a716-000000000150',  78,  51, 3405, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000151', '550e8400-e29b-41d4-a716-000000000151',   6,  37, 3220, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000152', '550e8400-e29b-41d4-a716-000000000152',  88,  71, 4639, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000153', '550e8400-e29b-41d4-a716-000000000153',  41,   1, 2567, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000154', '550e8400-e29b-41d4-a716-000000000154',  78,  87, 4122, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000155', '550e8400-e29b-41d4-a716-000000000155',  80,  81, 4813, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000156', '550e8400-e29b-41d4-a716-000000000156',  38,  24, 2335, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000157', '550e8400-e29b-41d4-a716-000000000157',  65,  65, 3929, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000158', '550e8400-e29b-41d4-a716-000000000158',  98,  91, 2555, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000159', '550e8400-e29b-41d4-a716-000000000159',  86,  62, 2990, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000160', '550e8400-e29b-41d4-a716-000000000160',  47,  94, 2550, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000161', '550e8400-e29b-41d4-a716-000000000161',  51,  34, 4579, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000162', '550e8400-e29b-41d4-a716-000000000162',  46,  49, 1170, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000163', '550e8400-e29b-41d4-a716-000000000163',  79,  49, 4694, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000164', '550e8400-e29b-41d4-a716-000000000164',  15,  20, 3209, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000165', '550e8400-e29b-41d4-a716-000000000165',  81,  44,  874, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000166', '550e8400-e29b-41d4-a716-000000000166',  20,  11, 1478, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000167', '550e8400-e29b-41d4-a716-000000000167',  89,  28,  890, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000168', '550e8400-e29b-41d4-a716-000000000168',  56,  82, 1578, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000169', '550e8400-e29b-41d4-a716-000000000169',  34,  52, 2120, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000170', '550e8400-e29b-41d4-a716-000000000170',  95,  95, 4978, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000171', '550e8400-e29b-41d4-a716-000000000171',  62,  34, 4342, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000172', '550e8400-e29b-41d4-a716-000000000172',  43,  28, 3812, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000173', '550e8400-e29b-41d4-a716-000000000173',  32,  40, 3278, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000174', '550e8400-e29b-41d4-a716-000000000174',  48,  54, 1415, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000175', '550e8400-e29b-41d4-a716-000000000175',  96,  40, 1886, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000176', '550e8400-e29b-41d4-a716-000000000176',  56,  71, 1073, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000177', '550e8400-e29b-41d4-a716-000000000177',  73,   2, 4768, 'Advanced'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000178', '550e8400-e29b-41d4-a716-000000000178',  98,  48, 4215, 'Intermediate'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000179', '550e8400-e29b-41d4-a716-000000000179',  89,  62, 3577, 'Beginner'),
('b2c3d4e5-f6a7-4b8c-9d0e-000000000180', '550e8400-e29b-41d4-a716-000000000180',  36,  92, 3932, 'Beginner');

-- =============================================================
-- DATA: users  (2 rows)
-- =============================================================
INSERT INTO public.users (user_id, email, hashed_password) VALUES
('820bad3a-58cd-4f6f-970c-11e7aca30b89', 'demo_user@sample.org', '$2a$10$OkM83.R6Ix/E.x0iLo9Z3u/KMTHbpJiG9fYLUAehKg1WgWW85ZL6q'),
('58d6bc7b-06a7-421e-8f38-4f0e9d09420d', 'john.doe@sample.org',  '$2a$10$fiNcWV8gRZAL1.WIi7qLvOhvZKvM4B8oywpiKtPbtauRbk5/YqvmW');

-- =============================================================
-- DATA: testsupport  (1 row — JWT token)
-- =============================================================
INSERT INTO public.testsupport (jwt) VALUES
('DE186661262016DE34991BDD6838195C17BC77B05E2BD2F83BA45D3F78B9A26A1449FDD1ADBE253CA7D19A730E76D8C4219F30F2460F3C3AE8BDA588B0C1EC84');

-- =============================================================
-- User - Setup test user with Read / Write
-- =============================================================
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'test_user') THEN
        CREATE USER test_user WITH PASSWORD 'tZVOnwMmfhv4gSbWcayiWRqN';
    ELSE
        ALTER USER test_user WITH PASSWORD 'tZVOnwMmfhv4gSbWcayiWRqN';
    END IF;
END
$$;

GRANT CONNECT ON DATABASE flyingshadow TO test_user;
GRANT USAGE ON SCHEMA public TO test_user;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO test_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO test_user;
-- =============================================================
-- Done
-- =============================================================
\echo 'flyingshadow database created and populated successfully.'