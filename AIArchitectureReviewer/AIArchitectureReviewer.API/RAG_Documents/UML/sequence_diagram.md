# Sequence Diagram

## Mô tả
Sequence diagram mô tả tương tác theo thời gian giữa các đối tượng/actor: message, call, return, lifeline.

## Khi nào dùng
- Thiết kế luồng xử lý use case hoặc tương tác giữa services.
- Mô tả orchestration và timing.

## Mermaid example
```mermaid
sequenceDiagram
    participant User
    participant API
    participant OrderService
    participant Inventory

    User->>API: POST /orders
    API->>OrderService: createOrder(data)
    OrderService->>Inventory: reserveItems(items)
    Inventory-->>OrderService: reserved
    OrderService-->>API: orderCreated
    API-->>User: 201 Created
```

## Checklist
- [ ] Xác định actors và participants chính.
- [ ] Thể hiện rõ luồng messages và response.
- [ ] Ghi chú các điều kiện async hoặc lỗi.

---

Người soạn: ArchReview AI — Sequence Diagram guide.