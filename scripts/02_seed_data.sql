-- =====================================================
-- RecipeHub Seed Data Script
-- =====================================================
-- This script populates the database with sample data for testing
-- Run this AFTER 01_create_database.sql

USE RecipeHubDb;

-- =====================================================
-- Insert Sample Users
-- =====================================================
-- NOTE: User profiles (Name, etc.) are stored in localStorage and Auth0
-- This table only stores the Auth0 mapping for reference
INSERT INTO Users (UserId, Email, Auth0Id) VALUES
('user-001', 'alice@example.com', 'auth0|user001'),
('user-002', 'bob@example.com', 'auth0|user002'),
('user-003', 'carol@example.com', 'auth0|user003')
ON DUPLICATE KEY UPDATE Email = VALUES(Email);

-- =====================================================
-- Insert Sample Recipes
-- =====================================================
INSERT INTO Recipes (RecipeId, Title, Description, Instructions, ImageUrl, MealDBId, Category, Area, Tags) VALUES
('recipe-001', 'Spaghetti Carbonara', 'Classic Italian pasta with eggs and bacon', 'Cook spaghetti. Fry bacon. Mix eggs with pasta water. Combine all ingredients. Season and serve.', 'https://www.themealdb.com/images/media/meals/xxrxux1503070723.jpg', '52891', 'Pasta', 'Italian', 'Pasta,Italian'),
('recipe-002', 'Chicken Tikka Masala', 'Creamy Indian chicken curry', 'Marinate chicken in yogurt and spices. Grill or bake. Make tomato cream sauce. Combine and simmer.', 'https://www.themealdb.com/images/media/meals/ushxrz1511722438.jpg', '52796', 'Curry', 'Indian', 'Curry,Spicy,Indian'),
('recipe-003', 'Beef Tacos', 'Mexican street-style tacos', 'Brown ground beef with spices. Warm tortillas. Add meat, lettuce, cheese, salsa. Serve with lime.', 'https://www.themealdb.com/images/media/meals/svvrxo1511723020.jpg', '52940', 'Seafood', 'Mexican', 'Tacos,Mexican')
ON DUPLICATE KEY UPDATE UpdatedAt = CURRENT_TIMESTAMP;

-- =====================================================
-- Insert Sample Favorites
-- =====================================================
INSERT INTO Favorites (FavoriteId, UserId, RecipeId) VALUES
('fav-001', 'user-001', 'recipe-001'),
('fav-002', 'user-001', 'recipe-002'),
('fav-003', 'user-002', 'recipe-001'),
('fav-004', 'user-003', 'recipe-003')
ON DUPLICATE KEY UPDATE CreatedAt = CreatedAt;

-- =====================================================
-- Insert Sample Contact Messages
-- =====================================================
INSERT INTO ContactMessages (ContactId, UserId, Email, Subject, Message, Status) VALUES
('contact-001', 'user-001', 'alice@example.com', 'Feature Request', 'Can you add a recipe rating system?', 'NEW'),
('contact-002', NULL, 'visitor@example.com', 'Bug Report', 'Found an issue with the search function', 'NEW'),
('contact-003', 'user-002', 'bob@example.com', 'Question', 'How can I export my favorites?', 'READ')
ON DUPLICATE KEY UPDATE UpdatedAt = CURRENT_TIMESTAMP;

-- =====================================================
-- Verify Data
-- =====================================================
SELECT 'Users:' as Section, COUNT(*) as Count FROM Users
UNION ALL
SELECT 'Recipes:' as Section, COUNT(*) as Count FROM Recipes
UNION ALL
SELECT 'Favorites:' as Section, COUNT(*) as Count FROM Favorites
UNION ALL
SELECT 'Contact Messages:' as Section, COUNT(*) as Count FROM ContactMessages;

COMMIT;
