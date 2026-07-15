# Interface Segregation (Refactoring hướng tới ISP)

## Mô tả
Khi một interface quá lớn hoặc nhiều client bị ép phải implement các phương thức không dùng, ta cần phân tách interface thành các interface nhỏ hơn — tương ứng với nguyên lý Interface Segregation (ISP).

## Khi nào refactor
- Interface có nhiều phương thức không liên quan.
- Client implement phần lớn phương thức nhưng không dùng một số phương thức khác.

## Cách refactor
1. Xác định các nhóm phương thức theo chức năng.
2. Tạo các interface nhỏ tương ứng (IPrinter, IScanner, ...).
3. Cập nhật các class implement để implement interface thích hợp hoặc compose nhiều interface.
4. Viết test đảm bảo behavior không thay đổi.

## Ví dụ (Java-like)
```java
// Before
interface MultifunctionDevice {
    void print(Document d);
    void scan(Document d);
    void fax(Document d);
}

class OldPrinter implements MultifunctionDevice {
    public void print(Document d) { /* ok */ }
    public void scan(Document d) { throw new UnsupportedOperationException(); }
    public void fax(Document d) { throw new UnsupportedOperationException(); }
}

// After: tách interface
interface Printer { void print(Document d); }
interface Scanner { void scan(Document d); }
interface Fax { void fax(Document d); }

class SimplePrinter implements Printer { public void print(Document d) { /*...*/ } }
```

## Checklist
- [ ] Interface có thể chia nhỏ thành các phần logic rõ ràng?
- [ ] Client không còn phải implement phương thức không dùng?
- [ ] Có giảm coupling và tăng testability không?

---

Người soạn: ArchReview AI — Hướng dẫn refactor theo ISP.