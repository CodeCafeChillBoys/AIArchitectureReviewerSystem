# Extract Method — Tách phương thức (Refactoring)

## Mô tả
Extract Method chuyển một đoạn code (thường trong phương thức dài) thành một phương thức mới có tên rõ ràng. Mục tiêu là làm cho code dễ đọc, tái sử dụng và dễ test.

## Khi nào dùng
- Phương thức quá dài hoặc có nhiều comment giải thích từng bước.
- Có khối lặp/điều kiện phức tạp cần tách riêng.
- Muốn tái sử dụng logic ở nhiều nơi.

## Các bước
1. Chọn đoạn code cần tách (không phụ thuộc quá nhiều vào biến cục bộ bên ngoài).
2. Tạo phương thức mới với tên miêu tả hành vi.
3. Thay đoạn code cũ bằng lời gọi phương thức mới.
4. Chạy test, điều chỉnh tham số/giá trị trả về nếu cần.

## Ví dụ (Python)
```python
# Before
def process_order(order):
    # validate
    if not order.items: raise ValueError('empty')
    # calculate total
    total = 0
    for i in order.items: total += i.price * i.qty
    # apply discount
    if order.coupon: total *= 0.9
    return total

# After: tách calculate_total và apply_discount
def calculate_total(order):
    return sum(i.price * i.qty for i in order.items)

def apply_discount(total, order):
    return total * 0.9 if order.coupon else total

def process_order(order):
    if not order.items: raise ValueError('empty')
    total = calculate_total(order)
    total = apply_discount(total, order)
    return total
```

## Checklist
- [ ] Đoạn tách nên có số ít phụ thuộc vào context bên ngoài.
- [ ] Tên phương thức rõ ràng, biểu diễn ý nghĩa.
- [ ] Viết test cho method mới nếu logic quan trọng.

## Lưu ý
- Nếu đoạn code cần nhiều biến cục bộ, xem xét tách thành class hoặc truyền context qua object.
- Extract Method là bước refactor đơn giản và an toàn — dùng thường xuyên.

---

Người soạn: ArchReview AI — Hướng dẫn Extract Method.