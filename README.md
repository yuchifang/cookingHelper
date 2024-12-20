# CookingHelper
這個專案是幫助料理的工具, 透過 Line bot建立機器人並與其互動達到紀錄庫存,食譜,購買清單的功能. 另外有後台系統可以查看 Line bot的使用狀況.

## 目錄
- [範例](#範例)
- [專案展示](#專案展示)
- [使用套件](#使用套件)




### 範例
**LineBot連結**: [https://line.me/R/ti/p/@438nxdys](https://line.me/R/ti/p/@438nxdys)  
> LintBot 沒有新增資料可以透過輸入 /data 產生假資料>

**後台連結**: [https://cookinghelper.azurewebsites.net/](https://cookinghelper.azurewebsites.net/)
> 管理者帳號:
> 帳號: cookinghelper@gmail.com
> 密碼: cookinghelper  
> 一般用戶帳號:
> 帳號: guest@gmail.com
> 密碼: guest01

### 專案展示
#### LintBot 使用介面
<div>
  <img src="github/images/lineBotMainCatalog.jpg" alt="LineBotMainCatalog" width="250"/>
</div>
透過點擊食譜清單,庫存管理,採買清單並依據提示,輸入文字或執行特定按鈕完成互動
<br/>
<br/>


| 食譜清單  | 庫存管理 | 採買清單|
| ------------- | ------------- | -------------|
| 沒有資料 | 沒有資料 | 沒有資料 |
|  <img src="github/images/RecipeListNoData.jpg" alt="RecipeListNoData" width="250"/> |  <img src="github/images/StorageManagementNoData.jpg" alt="StorageManagementNoData" width="250"/>  |  <img src="github/images/ShoppingListNoData.jpg" alt="ShoppingListNoData" width="250"/>  |
| 有資料  |有資料  | 有資料  |
|  <img src="github/images/RecipeList.jpg" alt="RecipeList" width="250"/>  |  <img src="github/images/StorageManagement.jpg" alt="StorageManagement" width="250"/>  |  <img src="github/images/ShoppingList.jpg" alt="ShoppingList" width="250"/> |

#### 後台連結
系統分析
<div>
<img src="github/images/AnalyzePage.png" alt="AnalyzePage" width="800"/>
</div>
<br/>
帳號管理

> 一般用戶不會有帳號管理這頁
<div>
<img src="github/images/AccountPage.png" alt="AccountPage" width="800"/>
</div>

### 使用套件
#### 前端
**@mui/material 6.1.4** 製作UI Component
<br/>
**react 18.2.0** 主要框架
<br/>
**react-router-dom 6.27.0** 製作網頁路由
<br/>
**vite 5.2.0**
<br/>
**typescript 5.2.2**
<br/>
**recharts 2.13.3, dayjs** 製作圖表
<br/>
**axios** 連接api

#### 後端
**Aspnet core net8**  主要框架
<br/>
**EntityFrameworkCore 8.0.6** ORM
<br/>
**Identity.EntityFrameworkCore** 用於帳號管理驗證
<br/>
**NUnit 3.0.0, Playwright.NUnit 1.49.0** 用於測試
<br/>
**line api 完成line 頁面**
