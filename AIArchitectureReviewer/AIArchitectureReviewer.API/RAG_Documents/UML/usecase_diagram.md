# Use Case Diagram

## Mô tả
Use case diagram mô tả các actors (user hệ thống) và các use cases (tính năng/hành vi) mà hệ thống cung cấp. Tốt để bắt đầu phân tích yêu cầu.

## Khi nào dùng
- Giai đoạn phân tích nghiệp vụ để thảo luận scope với stakeholders.

## Mermaid example
```mermaid
usecaseDiagram
    actor User
    actor Admin
    User --> (Place Order)
    User --> (Track Order)
    Admin --> (Manage Inventory)
```

## Checklist
- [ ] Xác định actors rõ ràng (user, system, external systems).
- [ ] Liệt kê use cases chính, nhóm theo chức năng.
- [ ] Tránh quá chi tiết — focus vào scope.

---

Người soạn: ArchReview AI — Use Case Diagram guide.