# Exam Management Platform Backend API

Exam Management Platform təhsil müəssisələri, kurs mərkəzləri və daxili korporativ təlim komandaları üçün hazırlanmış mərkəzləşdirilmiş imtahan və akademik idarəetmə sistemidir.

Bu sistem sadəcə test yaratmaq və nəticə hesablamaq üçün deyil, tam tədris prosesinin backend səviyyəsində idarə olunması üçün nəzərdə tutulub. Platforma istifadəçi rollarını, sinif strukturunu, müəllim-fənn əlaqələrini, tələbə iştirakını, imtahan sessiyalarını, cavabların emalını, təhlükəsizlik yoxlamalarını, davamiyyət proseslərini, tapşırıq axınlarını və statistik hesabatları vahid sistem altında birləşdirir.

Layihə real biznes mühitində istifadə oluna biləcək enterprise yanaşma ilə qurulub. Burada əsas məqsəd tədris müəssisəsində baş verən prosesləri manual idarəetmədən çıxarıb rəqəmsal, izlənə bilən və audit edilə bilən backend arxitekturasına çevirməkdir.

## Product Vision

Platformanın əsas məqsədi təhsil prosesində imtahanların, müəllim fəaliyyətlərinin, tələbə performansının və inzibati nəzarətin vahid mərkəzdən idarə olunmasını təmin etməkdir.

Sistem aşağıdakı real biznes problemlərini həll edir:

- imtahanların manual hazırlanması və nəticələrin əl ilə hesablanması problemini aradan qaldırır
- tələbə cavablarını sistemli şəkildə saxlayır və nəticələri avtomatik emal edir
- müəllim, tələbə, sinif və fənn əlaqələrini strukturlaşdırılmış formada idarə edir
- davamiyyət prosesini izlənə bilən və tarixçəli hala gətirir
- sistem daxilində baş verən kritik əməliyyatları audit log vasitəsilə qeyd edir
- exam access və security log mexanizmləri ilə imtahan təhlükəsizliyini artırır
- admin panel üçün statistik dashboard məlumatları yaradır
- gələcəkdə frontend, mobile app və cloud deployment üçün uyğun backend bazası təmin edir

## Business Process Overview

Sistemdə əsas proseslər bir-biri ilə əlaqəli şəkildə işləyir.

Admin əvvəlcə sistemdə əsas struktur məlumatlarını yaradır. Bu mərhələdə istifadəçilər, müəllimlər, tələbələr, siniflər və fənlər sistemə əlavə olunur. Daha sonra müəllimlər fənlərə, siniflərə və tədris proseslərinə bağlanır. Bu əlaqələr qurulduqdan sonra exam prosesi başlaya bilir.

İmtahan prosesi sadəcə exam yaratmaqdan ibarət deyil. Əvvəlcə exam metadata yaradılır. Daha sonra həmin exam üçün sual bankı formalaşdırılır, sualların cavab variantları əlavə olunur, düzgün cavablar qeyd edilir və exam müəyyən tələbə qrupuna və ya sinfə təyin edilir.

Student exam-a daxil olduqda sistem onun access hüququnu yoxlayır. Əgər tələbənin həmin exam üçün icazəsi varsa, ayrıca StudentExam sessiyası yaradılır. Bu sessiya tələbənin exam-a nə vaxt başladığını, hansı statusda olduğunu, hansı cavabları verdiyini və nəticədə neçə bal topladığını izləyir.

Cavablar göndərildikdən sonra sistem StudentAnswer və StudentAnswerOption məlumatlarını saxlayır. Daha sonra nəticə hesablama məntiqi işləyir və tələbənin nəticəsi sistemdə qalıcı şəkildə qeyd olunur. Bu məlumatlar sonradan dashboard, hesabat və performans analizi üçün istifadə oluna bilər.

## Advanced Business Logic

Sistem real təhsil idarəetməsi üçün bir neçə kompleks biznes axını dəstəkləyir.

### Role Based Operation Flow

Hər istifadəçi sistemdə eyni hüquqa sahib deyil.

Admin sistemin tam idarəsini həyata keçirir. Müəllim yalnız ona aid siniflər, fənlər, exam-lar və tələbələrlə işləyə bilir. Student isə yalnız öz profilinə, öz exam-larına, öz cavablarına və nəticələrinə baxa bilir.

Bu yanaşma sistemdə data isolation və security control təmin edir.

### Exam Lifecycle Flow

Exam yaradıldığı andan nəticə formalaşana qədər bir neçə mərhələdən keçir:

1. Draft mərhələsi  
2. Question preparation mərhələsi  
3. Assignment mərhələsi  
4. Access validation mərhələsi  
5. Active exam session mərhələsi  
6. Answer submission mərhələsi  
7. Result calculation mərhələsi  
8. Completed mərhələsi  
9. Audit və reporting mərhələsi  

Bu lifecycle sayəsində exam prosesi nəzarətsiz qalmır və hər mərhələ sistem tərəfindən izlənə bilir.

### Exam Security Flow

İmtahanlara giriş access code və token əsaslı yoxlama ilə qorunur.

Sistem tələbənin exam-a daxil olarkən icazəsini yoxlayır. Yanlış giriş cəhdi, uyğun olmayan token, icazəsiz exam request və digər kritik hallar security log-a yazıla bilir.

Bu, real online exam platformalarında vacib olan exam integrity prinsipini qorumağa kömək edir.

### Attendance Correction Flow

Davamiyyət sistemi sadəcə “gəldi / gəlmədi” məntiqindən ibarət deyil.

Müəllim attendance session yaradır, tələbələrin iştirak vəziyyətini qeyd edir və əgər dəyişiklik lazımdırsa, AttendanceChangeRequest mexanizmi vasitəsilə düzəliş sorğusu yaradıla bilir.

Bu yanaşma audit edilə bilən attendance workflow yaradır.

### Task and Notification Flow

Sistemdə müəllim və tələbə tapşırıqları ayrıca idarə olunur.

Yeni task yaradıldıqda və ya tələbəyə exam təyin edildikdə notification mexanizmi həmin istifadəçiyə məlumat ötürmək üçün istifadə oluna bilər.

Bu struktur gələcəkdə email, real-time notification və mobile push notification ilə genişləndirilə bilər.

## Enterprise Value

Bu backend sistemi aşağıdakı enterprise prinsiplərə əsaslanır:

- biznes məntiqi controller içində qarışdırılmayıb
- service layer vasitəsilə proseslər ayrılıb
- repository pattern ilə data access layer izolə olunub
- unit of work ilə transaction consistency qorunur
- JWT ilə stateless authentication qurulub
- audit log və security log ilə sistem izlənə bilir
- entity relation-lar real database modelinə uyğun qurulub
- dashboard service ilə idarəetmə səviyyəsində statistik məlumat hazırlanır

## Summary

Exam Management Platform backend API real təhsil mühitində istifadə oluna biləcək kompleks, genişlənə bilən və təhlükəsiz backend sistemidir.

Layihə imtahanların yaradılması, tələbə iştirakının izlənməsi, nəticələrin hesablanması, davamiyyətin idarə olunması, müəllim-tələbə-sinif əlaqələrinin qurulması və sistem hadisələrinin loglanması kimi prosesləri vahid arxitektura altında birləşdirir.

Bu sistem gələcəkdə frontend, mobile application, cloud deployment və microservice strukturu ilə daha böyük education management platformasına çevrilə bilər.
# System Architecture

Layihə Layered Architecture prinsipi ilə hazırlanıb. Məqsəd business logic, database access və presentation hissələrinin bir-birindən ayrılmasıdır.

Sistem aşağıdakı əsas qatlara bölünüb:

- API Layer
- Application Layer
- Domain Layer
- Infrastructure Layer

Bu struktur böyük sistemlərdə maintainability və scalability təmin etmək üçün istifadə olunur.

---

## API Layer

API layer presentation hissəsini idarə edir.

Burada yerləşir:

- Controllers
- Middleware
- Authentication configuration
- Swagger configuration
- Dependency Injection registration
- CORS configuration

Frontend bütün request-ləri bu qat vasitəsilə backend-ə göndərir.

Controller-lər business logic daşımır. Onlar request qəbul edib uyğun service-lərə yönləndirir.

Bu yanaşma controller-lərin sadə və idarə edilə bilən qalmasını təmin edir.

---

## Application Layer

Application layer sistemin əsas business logic hissəsidir.

Burada yerləşir:

- Services
- DTO objects
- Interfaces
- Validation logic
- Workflow management
- Business rules

Sistemdəki əsas proseslər burada idarə olunur.

Məsələn:

- exam creation flow
- attendance workflow
- result calculation
- notification handling
- authorization checks

hamısı application layer daxilində idarə olunur.

Bu yanaşma business logic-in controller və database hissəsindən ayrılmasını təmin edir.

---

## Domain Layer

Domain layer sistemin əsas entity modellərini saxlayır.

Burada yerləşir:

- Entities
- Enums
- Base entity structures
- Domain models

Bu qat sistemin database modelini və biznes strukturunu təmsil edir.

Məsələn:

- User
- Student
- Teacher
- Exam
- ExamQuestion
- StudentAnswer
- AttendanceRecord

kimi entity-lər burada yerləşir.

---

## Infrastructure Layer

Infrastructure layer database və xarici servis əlaqələrini idarə edir.

Burada yerləşir:

- DbContext
- Repository implementations
- Unit Of Work
- Migration files
- Hosted services
- Email services
- Persistence logic

Bu qat database əməliyyatlarını idarə edir və Entity Framework Core ilə işləyir.

---

# Database Design

Database relational struktur ilə hazırlanıb.

Entity-lər arasında foreign key və relation-lar qurulub.

Sistemdə aşağıdakı relation növləri istifadə olunur:

- One To Many
- Many To Many
- Self Relations

Məsələn:

- Teacher -> Subject
- Student -> ClassRoom
- Exam -> Questions
- Question -> Options
- Student -> Answers

kimi əlaqələr relational database prinsiplərinə uyğun qurulub.

Database dizaynında əsas məqsəd data consistency və scalable structure təmin etməkdir.

---

# Authentication and Authorization

Sistem JWT Authentication istifadə edir.

İstifadəçi login olduqdan sonra backend JWT token yaradır.

Bu token frontend tərəfindən saxlanılır və protected endpoint-lərə request göndərərkən istifadə olunur.

Sistem token vasitəsilə:

- istifadəçini tanıyır
- role məlumatını yoxlayır
- access permission müəyyən edir

Authorization hissəsi role-based structure ilə hazırlanıb.

Məsələn:

- Admin bütün endpoint-lərə daxil ola bilir
- Teacher yalnız öz proseslərini idarə edir
- Student yalnız öz məlumatlarını görə bilir

Bu yanaşma enterprise security prinsiplərinə uyğun hazırlanıb.

---

# Exam Management Logic

Exam sistemi layihənin əsas biznes hissəsidir.

Exam workflow aşağıdakı mərhələlərdən ibarətdir:

1. Exam creation
2. Question preparation
3. Option management
4. Student assignment
5. Access validation
6. Exam participation
7. Answer submission
8. Result calculation
9. Exam completion

Bu proseslər sistem daxilində ayrıca service-lər vasitəsilə idarə olunur.

---

## Question and Option Structure

Hər exam bir neçə sualdan ibarət ola bilər.

Hər sual üçün ayrıca option-lar yaradılır.

Question və option strukturu aşağıdakı üstünlükləri təmin edir:

- dynamic exam structure
- reusable question system
- answer validation
- automatic result calculation

---

## Student Participation Flow

Student exam-a daxil olduqda sistem ayrıca participation session yaradır.

Bu session aşağıdakı məlumatları izləyir:

- start time
- finish time
- status
- submitted answers
- calculated score

Bu yanaşma exam history və audit tracking üçün vacibdir.

---

# Attendance Management

Attendance sistemi tələbə davamiyyətinin idarə olunması üçün hazırlanıb.

Müəllim attendance session yaradır və tələbələrin statuslarını qeyd edir.

Əgər attendance dəyişiklikləri lazımdırsa:

- correction request
- approval workflow
- history tracking

mexanizmləri istifadə oluna bilir.

Bu sistem manual attendance problemlərini azaltmaq üçün hazırlanıb.

---

# Dashboard and Reporting

Dashboard sistemi admin panel üçün statistik məlumat hazırlayır.

Dashboard service aşağıdakı məlumatları hesablayır:

- total students
- total teachers
- total exams
- active sessions
- attendance statistics
- recent activities

Bu hissə gələcəkdə Power BI və advanced reporting sistemləri ilə genişləndirilə bilər.

---

# Notification System

Notification sistemi daxili sistem bildirişləri üçün istifadə olunur.

Məsələn:

- yeni exam assign edildikdə
- attendance dəyişdikdə
- task yaradıldıqda
- nəticə hazır olduqda

notification göndərilə bilər.

Bu sistem gələcəkdə:

- email notification
- realtime websocket notification
- mobile push notification

ilə genişləndirilə bilər.

---

# Audit and Security Logging

Sistem daxilində bütün kritik əməliyyatlar loglana bilir.

Audit və security logging aşağıdakı məqsədlər üçün istifadə olunur:

- suspicious activity detection
- user action tracking
- security monitoring
- debugging support
- historical tracking

Bu yanaşma enterprise security sistemlərində vacib hissələrdən biridir.

---

# Repository Pattern

Repository Pattern database əməliyyatlarını abstraction etmək üçün istifadə olunub.

Bu yanaşma business logic ilə persistence logic arasında ayrılıq yaradır.

Hər əsas entity üçün ayrıca repository mövcuddur.

Məsələn:

- UserRepository
- ExamRepository
- StudentRepository
- AttendanceRepository

Bu yanaşma kodun maintain edilməsini asanlaşdırır.

---

# Unit Of Work Pattern

Unit Of Work pattern transaction idarəsi üçün istifadə olunur.

Bu hissə:

- multiple repository operation
- transaction consistency
- centralized save operation

üçün vacibdir.

Bu yanaşma database consistency qorumağa kömək edir.

---

# Hosted Services

Sistem daxilində background hosted service istifadə olunub.

Bu service aşağıdakı proseslər üçün istifadə edilə bilər:

- automatic exam lifecycle update
- scheduled background tasks
- timeout handling
- cleanup operations

Bu yanaşma enterprise sistemlərdə background processing üçün istifadə olunur.

---

# Technologies Used

## ASP.NET Core Web API

Backend API development üçün istifadə olunub.

Üstünlükləri:

- yüksək performans
- scalability
- cross-platform support
- built-in dependency injection
- enterprise ecosystem

---

## Entity Framework Core

ORM kimi istifadə olunub.

Üstünlükləri:

- migration support
- LINQ query system
- relationship management
- database abstraction

---

## SQL Server

Əsas relational database sistemi kimi istifadə olunub.

Üstünlükləri:

- enterprise database support
- indexing support
- transaction management
- relational consistency

---

## JWT Authentication

Secure authentication sistemi üçün istifadə olunub.

Üstünlükləri:

- stateless authentication
- scalable structure
- frontend/backend separation
- secure token validation

---

## Swagger / OpenAPI

API documentation və testing üçün istifadə olunub.

Bu hissə development prosesini sürətləndirir və endpoint testing imkanları yaradır.

---

# Scalability Perspective

Layihə gələcəkdə genişlənə biləcək şəkildə hazırlanıb.

Hazırkı struktur aşağıdakı inkişaf istiqamətləri üçün uyğundur:

- React frontend integration
- Mobile application integration
- Cloud deployment
- Microservice migration
- Realtime systems
- Distributed architecture

Layered architecture və service separation səbəbilə sistem gələcəkdə rahat şəkildə genişləndirilə bilər.

---

# Conclusion

Exam Management Platform enterprise yanaşma ilə hazırlanmış kompleks backend sistemidir.

Layihə aşağıdakı əsas prosesləri vahid platformada birləşdirir:

- exam management
- attendance management
- user authorization
- student participation tracking
- result calculation
- notification workflows
- dashboard analytics
- audit logging

Sistem scalable, maintainable və security-oriented arxitektura ilə hazırlanıb və gələcəkdə daha böyük education management platformasına çevrilə bilər.
