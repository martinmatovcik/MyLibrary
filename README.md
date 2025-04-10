# 📚 MyLibrary
### 🌟 Overview
**MyLibrary** is a `domain-driven` library management system designed to facilitate item borrowing and rental processes. The system allows users to create orders, add items, and manage the entire rental lifecycle from creation to completion.  
### ✨ Features
- 📋 **Order Management:** Create, modify, and track orders through their lifecycle
- 🧩 **Item Management:** Add or remove items from orders
- 🔄 **Status Workflow:** Comprehensive order status system (`Created` → `Placed` → `Confirmed` → `Awaiting Pickup` → `Picked Up` → `Completed`)
- 📢 **Domain Events:** Rich domain events for integration with other systems
- ✅**Validation Rules:** Business rules enforced at the domain level
### 🏗️ Domain Model
#### 🛒 Order
The central entity that tracks a borrowing transaction. An order:  
- Contains multiple items (all from the same owner)
- Is associated with a renter (the person borrowing items)
- Has pickup and return dates
- Follows a defined status workflow
- Maintains its integrity through business rules
#### ⏱️ Order Lifecycle
1. 🆕 **Created:** Initial state when an order is first made
2. 📝 **Placed:** Order is submitted with pickup date and optional return date
3. 👍 **Confirmed:** Owner has approved the order
4. 👎 **Declined:** Owner has rejected the order _(Coming soon..)_
5. 🔜 **Awaiting Pickup:** Items are ready for collection
6. 🚚 **Picked Up:** Items have been collected by the renter
7. ✅ **Completed:** Items have been returned
8. ❌**Canceled:** Order has been canceled (can be recreated)
#### 📦 Item
A borrowable entity within the system:  
- Belongs to a specific owner
- Can be added to orders for rental
- Contains metadata about the borrowable item
- Can only be in one active order at a time
####  📖 Book
  A specialized type of borrowable item in the library system:
-  Contains standard book metadata (title, author, ISBN, publisher, etc.)
-  Can be categorized by genre, subject, or other classification
-  May have additional borrowing rules or restrictions
-  Has properties like publication date and edition information
-  Can be tracked for availability and location within the library
-  May contain condition information for physical copies
#### 👤 User
Users interact with the system in different roles:
- 👩‍ **Owners:** Users who make items available for borrowing
- 🧑‍ **Renters:** Users who borrow items through the order system
- Users are identified by unique identifiers
- Users can both lend and borrow items
### 🛠️ Technology Stack
- 🔤 **Language:** C#  
- 🧰 **Framework:** .NET  
- 🏛️ **Architecture:** Domain-Driven Design  
- ⏰ **Date/Time:** NodaTime library for proper date/time handling