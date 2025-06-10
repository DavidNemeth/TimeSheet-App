# TimeSheet Application - User Guide

Welcome to the TimeSheet Application! This guide will help you understand how to use the application to manage your timesheets effectively.

## Table of Contents

1.  [Getting Started](#getting-started)
    *   [Logging In](#logging-in)
2.  [Main Dashboard](#main-dashboard)
    *   [Navigating the Timesheet Grid](#navigating-the-timesheet-grid)
    *   [Filtering Entries](#filtering-entries)
    *   [Selecting a Month](#selecting-a-month)
3.  [Managing Timesheet Entries](#managing-timesheet-entries)
    *   [Adding a New Entry](#adding-a-new-entry)
    *   [Understanding the Timesheet Form](#understanding-the-timesheet-form)
    *   [Editing an Existing Entry](#editing-an-existing-entry)
    *   [Viewing Entry History](#viewing-entry-history)
4.  [Entry Statuses](#entry-statuses)
    *   [New](#new)
    *   [Approved](#approved)
    *   [Rejected](#rejected)
5.  [Approving or Rejecting Entries (for Supervisors/Admins)](#approving-or-rejecting-entries-for-supervisorsadmins)
6.  [Archiving Entries](#archiving-entries)
    *   [Archiving an Entry](#archiving-an-entry)
    *   [Viewing Archived Entries](#viewing-archived-entries)
    *   [Unarchiving an Entry](#unarchiving-an-entry)
7.  [Exporting Data](#exporting-data)
    *   [Exporting Overtime Data](#exporting-overtime-data)
    *   [Exporting Dirt Bonus Data](#exporting-dirt-bonus-data)
8.  [Troubleshooting](#troubleshooting)

---

## 1. Getting Started

### Logging In
(Details about the login process would go here. Assuming a standard username/password or SSO integration. This section might need more specific information based on the actual authentication setup which is not fully clear from the provided files yet, but we'll make a placeholder.)

To start using the application, open your web browser and navigate to the application's URL. You will be prompted to log in with your credentials.

---

## 2. Main Dashboard

After logging in, you will land on the main dashboard, which primarily displays your timesheet entries.

### Navigating the Timesheet Grid
The central part of the dashboard is the timesheet grid. It shows a list of your timesheet entries with several columns:
*   **Actions:** Buttons to Edit, View History, or Archive an entry.
*   **Emp-ID:** Your employee identification number.
*   **Created on:** The date the entry was created.
*   **Date:** The actual date the work was performed for.
*   **Employee:** Your username.
*   **OT:** Overtime hours recorded.
*   **Description (Overtime):** Details about the overtime work.
*   **DirtB:** Dirt bonus hours recorded.
*   **Description (Dirt Bonus):** Details about the dirt bonus.
*   **Status:** The current status of the entry (e.g., New, Approved, Rejected).
*   **Approved/Rejected by:** Who approved or rejected the entry.

### Filtering Entries
Each column in the grid usually has a search box directly underneath its title. You can type in these boxes to filter the entries based on the data in that column. For example, type a specific date in the "Date" filter to see entries for that day.

### Selecting a Month
Above the timesheet grid, you'll find a date picker. You can use this to select a specific month and year to view timesheet entries from that period.

---

## 3. Managing Timesheet Entries

### Adding a New Entry
1.  Click the "**Add New**" button, typically located above the timesheet grid.
2.  A dialog box titled "**New TimeSheet**" will appear.
3.  Fill in the required details in the [Timesheet Form](#understanding-the-timesheet-form).
4.  Click "**Save**" (or a similar button) to submit your new entry.

### Understanding the Timesheet Form
The timesheet form contains the following fields:
*   **Date:** The date for which you are logging time. Cannot be a future date. Special rules apply on the 1st of the month for logging previous month's entries.
*   **User Name:** Your name (usually pre-filled and read-only).
*   **Machine:** Select the relevant machine if applicable.
*   **Overtime:** Enter the number of overtime hours.
    *   **From/To:** Specify the start and end times for the overtime. These will auto-calculate or can be used to calculate the overtime hours.
*   **Overtime Payout:** Choose how you want your overtime to be compensated (e.g., "Pay out with the next salary," "Book to my time account").
*   **Description (Overtime):** Provide a brief description for the overtime claimed. This is required if overtime hours are entered.
*   **Dirtbonus:** Enter the number of dirt bonus hours. This must be in increments of 0.25 hours.
*   **Description (Dirt Bonus):** Provide a brief description for the dirt bonus claimed. This is required if dirt bonus hours are entered.
*   **Rejection Reason:** (Visible if an entry was rejected and you are editing it, or if you are a supervisor rejecting an entry) Displays or allows entry of the reason for rejection.
*   **Approved by/Rejected by:** (Visible for entries that have been processed) Shows who approved or rejected the entry.

### Editing an Existing Entry
1.  Find the entry you wish to edit in the timesheet grid.
2.  Click the **Edit** icon (often a pencil icon) in the "Actions" column for that entry.
3.  The timesheet form will appear, pre-filled with the entry's current details.
4.  Make your changes.
5.  Click "**Save**" to update the entry.

*Note: Whether you can edit an entry, and which fields you can edit, might depend on its current status and your user permissions (e.g., approved entries might have restrictions).*

### Viewing Entry History
To see the history of changes and status updates for an entry:
1.  Find the entry in the timesheet grid.
2.  Click the **History** icon (often a clock or list icon) in the "Actions" column.
3.  A dialog will appear showing the chronological history of the entry.

---

## 4. Entry Statuses

Timesheet entries can have the following statuses, indicated by a badge in the "Status" column:

### New
This is the default status for a newly created entry or an entry that has been edited after rejection. It means the entry is awaiting review. The badge is typically blue.

### Approved
The entry has been reviewed and approved by a supervisor or administrator. The badge is typically green. Approved entries are usually locked for editing unless you have special permissions.

### Rejected
The entry has been reviewed and rejected. The badge is typically red. You will usually find a rejection reason in the entry details. Rejected entries can often be edited and resubmitted (which changes their status back to "New").

---

## 5. Approving or Rejecting Entries (for Supervisors/Admins)

If you have the necessary permissions (e.g., you are a Team Head or have a role with approval rights):
1.  Open an entry that has a "**New**" status by clicking the **Edit** icon.
2.  Review the details of the timesheet entry.
3.  At the bottom of the form, you will see "**Approve**" and "**Reject**" buttons.
    *   Click "**Approve**" to approve the entry. The status will change to "Approved."
    *   Click "**Reject**" to reject the entry. You will likely need to provide a reason in the "**Rejection Reason**" field. The status will change to "Rejected."
4.  The changes are saved automatically.

---

## 6. Archiving Entries

### Archiving an Entry
If you need to remove an entry from the main "Active" view without deleting it permanently (and if you have permission):
1.  Find the entry in the "Active" timesheet grid.
2.  Click the **Archive** icon (often a box or archive symbol) in the "Actions" column.
3.  Confirm the action if prompted. The entry will be moved to the "Archived" section.

### Viewing Archived Entries
1.  On the main dashboard, look for a tab or filter labeled "**Archived**".
2.  Click this tab to switch to the view of archived timesheet entries.
3.  The grid will display entries that have been archived.

### Unarchiving an Entry
1.  Navigate to the "Archived" entries view.
2.  Find the entry you wish to restore.
3.  Click the **Unarchive** icon (often an arrow pointing out of a box or similar) in the "Actions" column.
4.  The entry will be moved back to the "Active" timesheet grid.

---

## 7. Exporting Data

If you have permission, you can export data for approved timesheets. Look for export buttons, typically near the "Add New" button.

### Exporting Overtime Data
1.  Click the "**Overtime**" export button.
2.  This will generate and download a CSV file containing details of approved overtime entries for the selected period. This is usually for payroll processing.

### Exporting Dirt Bonus Data
1.  Click the "**Dirtbonus**" export button.
2.  This will generate and download a CSV file containing details of approved dirt bonus entries for the selected period, often grouped by employee. This is usually for payroll processing.

*Note: Export buttons might be disabled if there are no approved entries with overtime or dirt bonus in the current view.*

---

## 8. Troubleshooting

*   **Cannot log in:** Double-check your username and password. If you've forgotten your password, look for a "Forgot Password" link or contact your administrator.
*   **Cannot find an entry:** Ensure you are looking in the correct month and that no filters are inadvertently hiding the entry. Check the "Archived" tab as well.
*   **"Add New" / "Edit" button is disabled or not visible:** You may not have the necessary permissions for this action, or there might be a temporary issue. Contact your administrator if you believe you should have access.
*   **Error message when saving:** Take note of the error message. It might indicate a required field is missing or data is in an incorrect format. If the problem persists, contact your administrator with the error details.

---
For further assistance, please contact your system administrator.
