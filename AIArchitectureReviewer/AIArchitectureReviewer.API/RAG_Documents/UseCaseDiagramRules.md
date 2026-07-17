# Quy tắc Đánh giá và Hướng dẫn Chi tiết Sơ đồ Ca sử dụng (Use Case Diagram)

Tài liệu này cung cấp định nghĩa chuẩn hóa, các khái niệm cấu trúc, ký pháp thành phần và quy tắc đánh giá chi tiết cho Sơ đồ Ca sử dụng (Use Case Diagram) dựa trên tiêu chuẩn UML.

---

## I. THÀNH PHẦN CHÍNH CỦA SƠ ĐỒ (USE CASE COMPONENTS)

Sơ đồ Ca sử dụng giúp trực quan hóa: Hệ thống đang được mô tả là gì (System), Ai đang dùng hệ thống (Actors), và Tác nhân muốn đạt được mục tiêu gì (Use Cases). Sơ đồ gồm 4 thành phần chính:

1. **Tác nhân (Actor)**:
   - Đại diện cho vai trò (role) của người dùng hoặc hệ thống bên ngoài tương tác với hệ thống. 
   - Có thể là con người hoặc hệ thống phần mềm/phần cứng ngoài khác.
   - Ký pháp: Hình người que (stick figure) hoặc hộp chữ nhật có stereotype `<<actor>>`.

2. **Ca sử dụng (Use Case)**:
   - Mô tả một chuỗi các hành động hoặc bước sự kiện xác định sự tương tác giữa Actor và Hệ thống để đạt được một mục tiêu nghiệp vụ cụ thể.
   - Bao gồm luồng sự kiện chính (Happy Path) và các luồng thay thế/ngoại lệ (Alternative/Exception Paths).
   - Ký pháp: Hình elip chứa tên Use Case (thường bắt đầu bằng động từ + danh từ).

3. **Ranh giới hệ thống (System Boundary)**:
   - Hộp chữ nhật bao quanh tất cả các Use Case để phân biệt những gì nằm bên trong hệ thống và những tác nhân ngoài nằm bên ngoài hệ thống.
   - Ký pháp: Hình hộp chữ nhật lớn có nhãn tên hệ thống ở trên cùng. Các Actor nằm ngoài hộp, các Use Case nằm trong hộp.

4. **Mối quan hệ (Relationship)**:
   - Kết nối giữa các Actor và Use Case, hoặc giữa các Use Case với nhau.

---

## II. CẤU TRÚC MỐI QUAN HỆ TRONG USE CASE (STRUCTURING USE CASES)

UML định nghĩa 4 mối quan hệ cốt lõi giữa các thành phần:

1. **Liên kết (Association)**:
   - Đường kết nối nét liền đơn giản, **không có mũi tên nhọn**, nối giữa một Actor và một Use Case mà họ có quyền thực hiện.

2. **Bao hàm (<<include>>)**:
   - Trích xuất các luồng hành vi chung/trùng lặp giữa nhiều Use Case thành một Use Case dùng chung để tái sử dụng.
   - Ca sử dụng gốc (Base Use Case) bắt buộc phải thực hiện ca sử dụng bao hàm (Included Use Case) để hoàn thành mục tiêu.
   - **Ký pháp**: Đường nét đứt có mũi tên nhọn trỏ từ **Base Use Case sang Included Use Case** kèm nhãn `<<include>>`.

3. **Mở rộng (<<extend>>)**:
   - Biểu diễn một luồng nghiệp vụ thay thế hoặc tùy chọn bổ sung cho Ca sử dụng gốc khi thỏa mãn điều kiện nhất định.
   - **Ký pháp**: Đường nét đứt có mũi tên nhọn **bắt buộc trỏ từ Extending Use Case (Mở rộng) về Base Use Case (Gốc)** kèm nhãn `<<extend>>`.

4. **Kế thừa / Khái quát hóa (Generalization)**:
   - Thể hiện quan hệ chuyên biệt hóa:
     - Giữa các Actor: `ActorCon` thừa kế quyền hạn của `ActorCha` (`ActorCon --|> ActorCha`).
     - Giữa các Use Case: `UCCon` là phiên bản đặc thù của `UCCha` (`UCCon --|> UCCha`). Use Case cha có thể được viết in nghiêng nếu là ca sử dụng trừu tượng (Abstract Use Case).
   - **Ký pháp**: Đường nét liền với đầu mũi tên hình tam giác rỗng.

---

## III. BẢN CHẤT NGHIỆP VỤ (BUSINESS vs ORDINARY USE CASES)

AI cần phân biệt mức độ đặc tả của sơ đồ:
- **Business Use Case**: Sử dụng thuật ngữ phi công nghệ để mô tả quy trình nghiệp vụ dưới dạng hộp đen thủ công/quy trình tổng quan (không nhất thiết phải được tự động hóa).
- **Ordinary Use Case**: Biểu diễn ở cấp độ chức năng phần mềm, chỉ rõ dịch vụ hoặc chức năng cụ thể mà hệ thống phần mềm cung cấp cho người dùng.

---

## IV. BỘ TIÊU CHUẨN KIỂM TRA LỖI KIẾN TRÚC CHO AI (CHECKLIST)

AI bắt buộc phải rà soát cấu trúc JSON và phân tích sơ đồ Use Case dựa trên các tiêu chuẩn kiểm định sau:

1. **Phân rã chức năng quá vụn vặt (Functional Decomposition)**:
   * **Dấu hiệu**: Vẽ các Use Case đại diện cho các bước lập trình, thao tác giao diện hoặc bước kỹ thuật tuần tự (ví dụ: "Nhấp chuột", "Nhập text vào ô", "Lưu Database", "Click button") thay vì các mục tiêu nghiệp vụ thực tế của Actor.
   * **Cách kiểm tra**: Quét tên Use Case. Nếu chứa các từ khóa kỹ thuật như "Click", "Save DB", "Input", "Nhấn nút", báo lỗi ngay.
   * **Giải pháp**: Yêu cầu gom nhóm các bước nhỏ thành một ca sử dụng nghiệp vụ hoàn chỉnh (ví dụ: thay vì "Nhập tên", "Nhập mật khẩu", "Click nút Đăng nhập" hãy gom thành ca sử dụng duy nhất "Đăng nhập").

2. **Dùng sai mối quan hệ <<include>> và <<extend>>**:
   * **Dấu hiệu**: 
     - Dùng `<<include>>` để mô tả trình tự thời gian (bước A xong đến bước B).
     - Chiều mũi tên của mối quan hệ `<<extend>>` bị vẽ ngược (trỏ từ gốc sang mở rộng).
   * **Cách kiểm tra**: Đối chiếu quan hệ giữa các Use Case. Kiểm tra nhãn và hướng mũi tên của các kết nối `<<extend>>` trong JSON.
   * **Giải pháp**: Yêu cầu đảo chiều mũi tên `<<extend>>` trỏ từ Use Case mở rộng về Use Case gốc. Đổi quan hệ tuần tự thành luồng sự kiện bên trong tài liệu đặc tả chứ không vẽ thành Use Case độc lập.

3. **Xác định sai đối tượng Actor**:
   * **Dấu hiệu**: Đưa cơ sở dữ liệu (Database), hệ thống nội bộ của phần mềm (ví dụ: "Hệ thống tự động", "Bản sao lưu") làm Actor. Actor bắt buộc phải là đối tượng nằm ngoài biên giới hệ thống và chủ động kích hoạt tương tác.
   * **Cách kiểm tra**: Quét mảng Actors. Nếu chứa các tên như "Database", "Hệ thống", "Hệ thống Backend", báo lỗi.
   * **Giải pháp**: Yêu cầu loại bỏ hoặc định nghĩa lại Actor đại diện cho người dùng hoặc hệ thống tích hợp bên ngoài thực sự.

4. **Sai hướng và định dạng mối liên kết (Association)**:
   * **Dấu hiệu**: Đường nối giữa Actor và Use Case có đầu mũi tên nhọn trỏ vào Use Case hoặc trỏ vào Actor (sai chuẩn UML).
   * **Cách kiểm tra**: Đọc cấu trúc quan hệ giữa Actor và Use Case.
   * **Giải pháp**: Yêu cầu đổi thành đường nét liền không có đầu mũi tên (Association đơn giản).