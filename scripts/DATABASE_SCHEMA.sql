-- ============================================================================
-- MenuMalin - Complete Clean Database Schema
-- Generated from Entity Framework Core DbContext
-- MySQL Version: 8.0+
-- Charset: utf8mb4 (for emoji and special characters)
-- ============================================================================

-- ⚠️  WARNING: This script will DELETE all existing data!
-- Make sure you have backups before running this!

-- ============================================================================
-- STEP 1: DROP AND RECREATE DATABASE
-- ============================================================================

DROP DATABASE IF EXISTS recipehubbdd;

CREATE DATABASE recipehubbdd
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE recipehubbdd;

-- ============================================================================
-- TABLE 1: Users (Auth0 Integration)
-- ============================================================================
-- Stores user accounts linked to Auth0 authentication
-- Profile data (Name, Picture) stays in Auth0 or localStorage

CREATE TABLE Users (
  UserId VARCHAR(36) NOT NULL PRIMARY KEY COMMENT 'Unique UUID identifier',
  Auth0Id VARCHAR(255) NOT NULL UNIQUE COMMENT 'Auth0 subject identifier',
  Email VARCHAR(255) NOT NULL UNIQUE COMMENT 'User email',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Registration timestamp',

  INDEX IX_Users_Auth0Id (Auth0Id),
  INDEX IX_Users_Email (Email)
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='User accounts synchronized with Auth0';

-- ============================================================================
-- TABLE 2: Recipes (TheMealDB API Cache)
-- ============================================================================
-- Stores recipes from TheMealDB API
-- Can also store user-favorite metadata

CREATE TABLE Recipes (
  RecipeId VARCHAR(36) NOT NULL PRIMARY KEY COMMENT 'Unique UUID identifier',
  Title VARCHAR(500) NOT NULL COMMENT 'Recipe name',
  Description LONGTEXT COMMENT 'Recipe description',
  Instructions LONGTEXT NOT NULL COMMENT 'Cooking instructions',
  ImageUrl LONGTEXT COMMENT 'Recipe image URL',
  MealDBId VARCHAR(50) NOT NULL UNIQUE COMMENT 'TheMealDB API ID',
  Category VARCHAR(100) COMMENT 'Recipe category (Pasta, Dessert, etc)',
  Area VARCHAR(100) COMMENT 'Cuisine area (Italian, French, etc)',
  Tags VARCHAR(500) COMMENT 'Recipe tags',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'When added to DB',
  UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT 'Last updated',

  UNIQUE INDEX IX_Recipes_MealDBId (MealDBId),
  INDEX IX_Recipes_Category (Category),
  INDEX IX_Recipes_Title (Title),
  INDEX IX_Recipes_Area (Area)
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Public recipes from TheMealDB API';

-- ============================================================================
-- TABLE 3: Favorites (User Favorite Recipes)
-- ============================================================================
-- Many-to-many relationship between Users and Recipes
-- Each user can favorite each recipe only once

CREATE TABLE Favorites (
  FavoriteId VARCHAR(36) NOT NULL PRIMARY KEY COMMENT 'Unique UUID identifier',
  UserId VARCHAR(36) NOT NULL COMMENT 'Reference to Users',
  RecipeId VARCHAR(36) NOT NULL COMMENT 'Reference to Recipes',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'When favorited',

  UNIQUE INDEX IX_Favorites_UserRecipe (UserId, RecipeId),
  INDEX IX_Favorites_UserId (UserId),
  INDEX IX_Favorites_RecipeId (RecipeId),

  CONSTRAINT FK_Favorites_Users_UserId
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
    ON DELETE CASCADE ON UPDATE CASCADE,

  CONSTRAINT FK_Favorites_Recipes_RecipeId
    FOREIGN KEY (RecipeId) REFERENCES Recipes(RecipeId)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='User favorite recipes';

-- ============================================================================
-- TABLE 4: UserRecipes (User-Created Custom Recipes)
-- ============================================================================
-- Stores recipes created by authenticated users
-- Can be marked public for community sharing

CREATE TABLE UserRecipes (
  UserRecipeId VARCHAR(36) NOT NULL PRIMARY KEY COMMENT 'Unique UUID identifier',
  UserId VARCHAR(36) NOT NULL COMMENT 'Creator Auth0 ID (foreign key)',
  Title VARCHAR(200) NOT NULL COMMENT 'Recipe title',
  IngredientsJson VARCHAR(2000) COMMENT 'Ingredients as JSON',
  IsPublic BOOLEAN NOT NULL DEFAULT FALSE COMMENT 'Public visibility flag',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Creation timestamp',
  UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT 'Last update',

  INDEX IX_UserRecipes_UserId (UserId),
  INDEX IX_UserRecipes_IsPublic (IsPublic),

  CONSTRAINT FK_UserRecipes_Users_Auth0Id
    FOREIGN KEY (UserId) REFERENCES Users(Auth0Id)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='User-created recipes';

-- ============================================================================
-- TABLE 5: ContactMessages (Contact Form Submissions)
-- ============================================================================
-- Stores contact form messages from users and anonymous visitors
-- Optional user reference (NULL for anonymous)

CREATE TABLE ContactMessages (
  ContactId VARCHAR(36) NOT NULL PRIMARY KEY COMMENT 'Unique UUID identifier',
  UserId VARCHAR(36) COMMENT 'Optional reference to Users (NULL if anonymous)',
  Email VARCHAR(255) NOT NULL COMMENT 'Contact email address',
  Subject VARCHAR(500) NOT NULL COMMENT 'Message subject',
  Message LONGTEXT NOT NULL COMMENT 'Message content',
  Status INT NOT NULL DEFAULT 0 COMMENT '0=New, 1=Read, 2=Responded, 3=Archived',
  CreatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) COMMENT 'Submission timestamp',
  UpdatedAt DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6) COMMENT 'Last update',

  INDEX IX_ContactMessages_UserId (UserId),
  INDEX IX_ContactMessages_Email (Email),
  INDEX IX_ContactMessages_Status (Status),
  INDEX IX_ContactMessages_CreatedAt (CreatedAt),

  CONSTRAINT FK_ContactMessages_Users_UserId
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
    ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Contact form messages';

-- ============================================================================
-- TABLE 6: __EFMigrationsHistory (Entity Framework Core Tracking)
-- ============================================================================
-- Required by Entity Framework Core to track database migrations

CREATE TABLE __EFMigrationsHistory (
  MigrationId VARCHAR(150) NOT NULL PRIMARY KEY,
  ProductVersion VARCHAR(32) NOT NULL
) ENGINE=InnoDB
  DEFAULT CHARSET=utf8mb4
  COLLATE=utf8mb4_unicode_ci
  COMMENT='Entity Framework Core migrations history';

-- ============================================================================
-- VERIFICATION
-- ============================================================================
-- Show all created tables
SHOW TABLES;

-- Show table structures
DESCRIBE Users;
DESCRIBE Recipes;
DESCRIBE Favorites;
DESCRIBE UserRecipes;
DESCRIBE ContactMessages;

-- ============================================================================
-- ✅ DATABASE SETUP COMPLETE!
-- ============================================================================
-- The menuMalin database is now clean and ready for use
-- - All tables created with proper relationships
-- - Foreign keys configured with CASCADE rules
-- - Indexes optimized for queries
-- - Character set: utf8mb4 (emoji & international support)
