# Hướng dẫn đưa code của các nhóm lên GitHub

## 1. Mục tiêu

Tài liệu này hướng dẫn các nhóm đưa code lên repository chung của dự án:

```txt
training-center-management
```

Dự án được tổ chức theo mô hình monorepo, gồm nhiều thư mục riêng cho từng nhóm:

```txt
training-center-management/
│
├── docs/
│
├── frontend/
│   └── vue-client/
│
├── gateway/
│
└── services/
    ├── course-schedule-service/
    ├── student-attendance-service/
    └── payment-report-service/
```

Mỗi nhóm chỉ được làm việc trong thư mục được phân công để tránh ghi đè code của nhóm khác.

---

## 2. Phân công thư mục

| Nhóm                                   | Thư mục được làm việc                     |
| -------------------------------------- | ----------------------------------------- |
| Nhóm 1 - Course & Schedule Service     | `services/course-schedule-service/`       |
| Nhóm 2 - Student & Attendance Service  | `services/student-attendance-service/`    |
| Nhóm 3 - Payment & Report/Auth Service | `services/payment-report-service/`        |
| Nhóm Frontend                          | `frontend/vue-client/`                    |
| Nhóm Gateway, nếu có                   | `gateway/`                                |
| PM/Lead                                | `docs/`, `README.md`, file cấu hình chung |

Không tự ý sửa thư mục của nhóm khác.

Ví dụ:

* Nhóm Course không sửa `services/student-attendance-service/`
* Nhóm Frontend không sửa code trong `services/`
* Nhóm Payment không sửa code của Course hoặc Student

---

## 3. Quy tắc branch

Repository sử dụng các branch chính:

```txt
main      : branch ổn định, dùng để demo hoặc nộp bài
develop   : branch tích hợp code của các nhóm
```

Các nhóm tạo branch riêng để làm việc:

```txt
service/course-schedule
service/student-attendance
service/payment-report
frontend/vue-client
gateway/ocelot
```

Quy tắc:

```txt
Không push trực tiếp vào main.
Không push trực tiếp vào develop nếu chưa được phép.
Mỗi nhóm tạo branch riêng.
Code xong thì tạo Pull Request vào develop.
PM/Lead kiểm tra rồi merge.
```

---

## 4. Lần đầu clone repo

Mỗi nhóm clone repository chung về máy:

```bash
git clone https://github.com/<username-or-org>/training-center-management.git
cd training-center-management
```

Sau đó chuyển sang branch `develop`:

```bash
git checkout develop
git pull origin develop
```

Nếu repo chưa có branch `develop`, báo PM/Lead tạo trước.

---

## 5. Nhóm 1 - Course & Schedule Service

### 5.1 Tạo branch

```bash
git checkout develop
git pull origin develop
git checkout -b service/course-schedule
```

### 5.2 Copy code

Copy toàn bộ source code của Course & Schedule Service vào thư mục:

```txt
services/course-schedule-service/
```

Nếu trong thư mục có file `.gitkeep`, hãy xóa file đó sau khi đã copy code thật vào.

### 5.3 Kiểm tra trạng thái Git

```bash
git status
```

Chỉ nên thấy thay đổi trong:

```txt
services/course-schedule-service/
```

### 5.4 Commit và push

```bash
git add services/course-schedule-service
git commit -m "Add course schedule service"
git push origin service/course-schedule
```

### 5.5 Tạo Pull Request

Trên GitHub tạo Pull Request:

```txt
base: develop
compare: service/course-schedule
```

---

## 6. Nhóm 2 - Student & Attendance Service

### 6.1 Tạo branch

```bash
git checkout develop
git pull origin develop
git checkout -b service/student-attendance
```

### 6.2 Copy code

Copy toàn bộ source code của Student & Attendance Service vào thư mục:

```txt
services/student-attendance-service/
```

Nếu trong thư mục có file `.gitkeep`, hãy xóa file đó sau khi đã copy code thật vào.

### 6.3 Kiểm tra trạng thái Git

```bash
git status
```

Chỉ nên thấy thay đổi trong:

```txt
services/student-attendance-service/
```

### 6.4 Commit và push

```bash
git add services/student-attendance-service
git commit -m "Add student attendance service"
git push origin service/student-attendance
```

### 6.5 Tạo Pull Request

Trên GitHub tạo Pull Request:

```txt
base: develop
compare: service/student-attendance
```

---

## 7. Nhóm 3 - Payment & Report/Auth Service

### 7.1 Tạo branch

```bash
git checkout develop
git pull origin develop
git checkout -b service/payment-report
```

### 7.2 Copy code

Copy toàn bộ source code của Payment & Report/Auth Service vào thư mục:

```txt
services/payment-report-service/
```

Nếu trong thư mục có file `.gitkeep`, hãy xóa file đó sau khi đã copy code thật vào.

### 7.3 Kiểm tra trạng thái Git

```bash
git status
```

Chỉ nên thấy thay đổi trong:

```txt
services/payment-report-service/
```

### 7.4 Commit và push

```bash
git add services/payment-report-service
git commit -m "Add payment report auth service"
git push origin service/payment-report
```

### 7.5 Tạo Pull Request

Trên GitHub tạo Pull Request:

```txt
base: develop
compare: service/payment-report
```

---

## 8. Nhóm Frontend

### 8.1 Tạo branch

```bash
git checkout develop
git pull origin develop
git checkout -b frontend/vue-client
```

### 8.2 Copy code

Copy toàn bộ source code VueJS frontend vào thư mục:

```txt
frontend/vue-client/
```

Nếu trong thư mục có file `.gitkeep`, hãy xóa file đó sau khi đã copy code thật vào.

### 8.3 Kiểm tra trạng thái Git

```bash
git status
```

Chỉ nên thấy thay đổi trong:

```txt
frontend/vue-client/
```

### 8.4 Commit và push

```bash
git add frontend/vue-client
git commit -m "Add Vue frontend"
git push origin frontend/vue-client
```

### 8.5 Tạo Pull Request

Trên GitHub tạo Pull Request:

```txt
base: develop
compare: frontend/vue-client
```

---

## 9. Sau khi Pull Request được merge

Sau khi PM/Lead merge Pull Request vào `develop`, mỗi nhóm cần cập nhật code mới nhất:

```bash
git checkout develop
git pull origin develop
```

Sau đó tạo branch mới để làm chức năng tiếp theo.

Ví dụ:

```bash
git checkout -b feature/course-crud
```

Hoặc:

```bash
git checkout -b feature/student-enrollment
```

Hoặc:

```bash
git checkout -b feature/frontend-login
```

---

## 10. Quy trình làm feature mới

Mỗi lần làm một chức năng mới, làm theo quy trình:

```bash
git checkout develop
git pull origin develop
git checkout -b feature/<ten-chuc-nang>
```

Ví dụ:

```bash
git checkout -b feature/student-import
```

Sau khi code xong:

```bash
git status
git add <thu-muc-cua-nhom>
git commit -m "Implement student import"
git push origin feature/student-import
```

Sau đó tạo Pull Request vào `develop`.

---

## 11. Quy tắc đặt tên branch

Nên đặt tên branch rõ ràng:

```txt
service/course-schedule
service/student-attendance
service/payment-report
frontend/vue-client

feature/course-crud
feature/class-schedule
feature/student-crud
feature/student-enrollment
feature/attendance
feature/result-input
feature/invoice-payment
feature/frontend-login
feature/frontend-dashboard
```

Không nên đặt branch kiểu:

```txt
test
abc
code-moi
sua-loi
branch-cua-toi
```

---

## 12. Quy tắc commit message

Commit message nên ngắn gọn, rõ việc đã làm.

Ví dụ tốt:

```txt
Add course schedule service
Implement student CRUD APIs
Implement enrollment approval
Add payment invoice APIs
Add Vue login page
Fix attendance validation
Update frontend route guard
```

Không nên commit kiểu:

```txt
update
fix
code
done
abc
test
```

---

## 13. Những file không nên commit

Không commit các file chứa thông tin cá nhân hoặc cấu hình máy riêng:

```txt
.env
.env.local
appsettings.Local.json
appsettings.Development.local.json
bin/
obj/
node_modules/
dist/
.vs/
.idea/
```

Nếu cần chia sẻ cấu hình mẫu, tạo file:

```txt
.env.example
appsettings.Example.json
```

Ví dụ `.env.example` cho frontend:

```env
VITE_API_BASE_URL=http://localhost:7000
```

Ví dụ `appsettings.Example.json` cho backend:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ServiceDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "TrainingCenter",
    "Audience": "TrainingCenterUsers",
    "SecretKey": "CHANGE_THIS_SECRET_KEY"
  }
}
```

---

## 14. Kiểm tra trước khi push

Trước khi push, mỗi nhóm cần kiểm tra:

```bash
git status
```

Đảm bảo chỉ có file trong thư mục nhóm mình.

Ví dụ nhóm Frontend chỉ nên thấy:

```txt
frontend/vue-client/...
```

Ví dụ nhóm Course chỉ nên thấy:

```txt
services/course-schedule-service/...
```

Nếu thấy file của nhóm khác bị thay đổi, cần kiểm tra lại trước khi commit.

---

## 15. Cách xử lý khi bị conflict

Nếu Pull Request báo conflict, không tự merge bừa.

Cách xử lý:

```bash
git checkout <branch-cua-ban>
git pull origin develop
```

Sau đó mở file conflict, sửa lại, rồi:

```bash
git add .
git commit -m "Resolve merge conflict"
git push origin <branch-cua-ban>
```

Nếu conflict nằm trong file của nhóm khác, báo PM/Lead xử lý.

---

## 16. Cách cập nhật code mới nhất từ develop

Trước khi bắt đầu code mỗi ngày, nên chạy:

```bash
git checkout develop
git pull origin develop
```

Sau đó mới tạo branch feature mới:

```bash
git checkout -b feature/<ten-chuc-nang>
```

Nếu đang ở branch feature và muốn cập nhật thay đổi mới từ develop:

```bash
git checkout feature/<ten-chuc-nang>
git pull origin develop
```

---

## 17. Quy tắc làm việc với tài liệu

Tài liệu dự án nằm trong:

```txt
docs/
```

Chỉ PM/Lead hoặc người được phân công mới sửa file trong `docs/`.

Nếu nhóm backend muốn thay đổi API contract, cần báo PM/Lead trước.

Không tự ý đổi endpoint, request body hoặc response body sau khi frontend đã code theo contract.

---

## 18. Quy tắc làm việc với API contract

Sau khi API contract đã chốt:

```txt
Không tự ý đổi endpoint.
Không tự ý đổi tên field.
Không tự ý đổi kiểu dữ liệu.
Không tự ý đổi format response.
```

Nếu cần bổ sung chức năng:

```txt
Ưu tiên thêm endpoint mới.
Ưu tiên thêm field optional.
Không xóa field cũ.
Không đổi ý nghĩa status cũ.
```

Ví dụ không nên đổi:

```json
{
  "classId": 1,
  "className": "English Basic 01"
}
```

thành:

```json
{
  "id": 1,
  "name": "English Basic 01"
}
```

Vì frontend có thể đã code theo `classId` và `className`.

---

## 19. Pull Request checklist

Trước khi tạo Pull Request, tự kiểm tra:

```txt
Code nằm đúng thư mục nhóm mình
Không sửa nhầm thư mục nhóm khác
Không commit node_modules, bin, obj, dist
Không commit secret thật
Đã build/run thử project
Đã cập nhật README nếu cần
Commit message rõ ràng
Pull Request merge vào develop, không merge thẳng vào main
```

---

## 20. Quy trình tổng kết

Quy trình chuẩn cho mỗi nhóm:

```txt
1. Clone repo
2. Checkout develop
3. Pull code mới nhất
4. Tạo branch riêng
5. Copy/code trong thư mục nhóm mình
6. Kiểm tra git status
7. Add đúng thư mục nhóm mình
8. Commit
9. Push branch
10. Tạo Pull Request vào develop
11. Chờ review và merge
```

---

## 21. Lệnh Git thường dùng

### Xem branch hiện tại

```bash
git branch
```

### Xem trạng thái file

```bash
git status
```

### Lấy code mới nhất

```bash
git pull origin develop
```

### Tạo branch mới

```bash
git checkout -b feature/<ten-chuc-nang>
```

### Chuyển branch

```bash
git checkout <ten-branch>
```

### Add file

```bash
git add <duong-dan>
```

### Commit

```bash
git commit -m "Commit message"
```

### Push

```bash
git push origin <ten-branch>
```

---

## 22. Lưu ý cuối

Repository chung giúp các nhóm làm việc song song, nhưng phải tuân thủ quy tắc:

```txt
Mỗi nhóm chỉ sửa thư mục của mình.
Không push trực tiếp vào main.
Tất cả thay đổi phải qua Pull Request.
Không tự ý đổi API contract.
Luôn pull develop mới nhất trước khi tạo branch mới.
```

Làm đúng quy trình này thì 4 nhóm có thể code cùng lúc mà không ảnh hưởng tới nhau.

```
```
