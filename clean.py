import os
import shutil
import tempfile

# === CONFIGURAÇÃO ===
SOLUTION_PATH = r"C:\visual_studio\SmartFactorySolution"

# Pastas a remover dentro de cada projeto
TARGET_FOLDERS = ["bin", "obj"]


def delete_folder(path):
    if os.path.exists(path):
        try:
            shutil.rmtree(path)
            print(f"[OK] Apagado: {path}")
        except Exception as e:
            print(f"[ERRO] Não foi possível apagar {path}: {e}")


def clean_bin_obj(solution_path):
    print("🔍 A procurar projetos...")
    for root, dirs, files in os.walk(solution_path):
        for folder in TARGET_FOLDERS:
            if folder in dirs:
                delete_folder(os.path.join(root, folder))


def clean_temp_aspnet():
    temp_dir = os.path.join(
        os.environ.get("TEMP", tempfile.gettempdir()),
        "Temporary ASP.NET Files"
    )

    print("\n🧹 A limpar Temporary ASP.NET Files...")
    if os.path.exists(temp_dir):
        for item in os.listdir(temp_dir):
            delete_folder(os.path.join(temp_dir, item))
    else:
        print("[INFO] Pasta Temporary ASP.NET Files não encontrada.")


if __name__ == "__main__":
    print("🚀 Início da limpeza...\n")

    clean_bin_obj(SOLUTION_PATH)
    clean_temp_aspnet()

    print("\n✅ Limpeza concluída!")
    print("👉 Agora abre o Visual Studio e faz: Rebuild Solution")
