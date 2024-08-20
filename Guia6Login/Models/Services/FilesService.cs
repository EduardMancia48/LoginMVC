
using Firebase.Auth;
using Firebase.Storage;

namespace Guia6Login.Models.Services
{
    public class FilesService : IFileHelper
    {
      
            public async Task<string> SubirArchivo(Stream archivo, string nombre)
            {
                string email = "eduardomancia48@gmail.com";
                string clave = "terrestre";
                string ruta = "login-4d670.appspot.com";
                string api_key = "AIzaSyC0RM4US7_QI4b2Mv-OGsDNAAZDasZKapc";

                var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));

                var a = await auth.SignInWithEmailAndPasswordAsync(email, clave);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    ruta,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })

                    .Child("Fotos_Perfil")
                    .Child(nombre)

                    .PutAsync(archivo, cancellation.Token);

                var downloadURL = await task;

                return downloadURL;

            }
        }
    }

