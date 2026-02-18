-- =====================================================
-- RecipeHub Database Creation Script
-- =====================================================
-- This script creates the complete database schema for the RecipeHub application
-- Run this script first to initialize the database

-- Drop existing database if needed (uncomment to use)
-- DROP DATABASE IF EXISTS RecipeHubDb;

-- Create the database
CREATE DATABASE IF NOT EXISTS RecipeHubDb;
USE RecipeHubDb;

-- =====================================================
-- Users Table
-- =====================================================
-- Lightweight user table (hybrid approach with localStorage)
-- Stores only Auth0 mapping and email for reference
-- Full profile (Name, etc.) is stored in localStorage and Auth0
CREATE TABLE IF NOT EXISTS Users (
    UserId VARCHAR(36) PRIMARY KEY,
    Auth0Id VARCHAR(255) NOT NULL UNIQUE,
    Email VARCHAR(255) NOT NULL UNIQUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_auth0id (Auth0Id),
    INDEX idx_email (Email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- Recipes Table
-- =====================================================
-- Stores recipe data from TheMealDB and user information
CREATE TABLE IF NOT EXISTS Recipes (
    RecipeId VARCHAR(36) PRIMARY KEY,
    Title VARCHAR(500) NOT NULL,
    Description LONGTEXT,
    Instructions LONGTEXT NOT NULL,
    ImageUrl VARCHAR(500),
    MealDBId VARCHAR(50) NOT NULL UNIQUE,
    Category VARCHAR(100),
    Area VARCHAR(100),
    Tags VARCHAR(500),
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_mealdbid (MealDBId),
    INDEX idx_category (Category),
    INDEX idx_title (Title(100)),
    FULLTEXT INDEX ft_title_description (Title, Description)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- Favorites Table
-- =====================================================
-- Stores user favorites (many-to-many relationship between Users and Recipes)
CREATE TABLE IF NOT EXISTS Favorites (
    FavoriteId VARCHAR(36) PRIMARY KEY,
    UserId VARCHAR(36) NOT NULL,
    RecipeId VARCHAR(36) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY unique_user_recipe (UserId, RecipeId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    FOREIGN KEY (RecipeId) REFERENCES Recipes(RecipeId) ON DELETE CASCADE,
    INDEX idx_userid (UserId),
    INDEX idx_recipeid (RecipeId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- Contact Messages Table
-- =====================================================
-- Stores contact form submissions (supports both authenticated and anonymous)
CREATE TABLE IF NOT EXISTS ContactMessages (
    ContactId VARCHAR(36) PRIMARY KEY,
    UserId VARCHAR(36) NULL,
    Email VARCHAR(255) NOT NULL,
    Subject VARCHAR(500) NOT NULL,
    Message LONGTEXT NOT NULL,
    Status ENUM('NEW', 'READ', 'RESPONDED', 'ARCHIVED') DEFAULT 'NEW',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    INDEX idx_userid (UserId),
    INDEX idx_email (Email),
    INDEX idx_status (Status),
    INDEX idx_createdat (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =====================================================
-- Indexes for Performance
-- =====================================================
-- Additional indexes for common queries

-- Favorites queries
ALTER TABLE Favorites ADD INDEX idx_created (CreatedAt);

-- Recipes queries
ALTER TABLE Recipes ADD INDEX idx_updated (UpdatedAt);

-- Contact queries
ALTER TABLE ContactMessages ADD INDEX idx_email_created (Email, CreatedAt);

-- =====================================================
-- Initial Data (Optional)
-- =====================================================
-- You can add initial data here if needed

-- Example: Insert a test user (uncomment to use)
-- INSERT INTO Users (UserId, Email, Name, Auth0Id)
-- VALUES ('user-001', 'test@example.com', 'Test User', 'auth0|123456');

COMMIT;
