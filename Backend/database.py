import pyodbc

def get_connection():
    conn_str = (
        "Driver={SQL Server};"
        "Server=DESKTOP-Q6N4J46\\SQLEXPRESS;"
        "Database=WebDocTruyen;"
        "Trusted_Connection=yes"
    )
    return pyodbc.connect(conn_str)