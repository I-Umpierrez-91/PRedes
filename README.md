# PRedes
 
## Pendientes

- Diagramas
- Documentacion


- Cambiar logs para que dependan de una variable.

## Documentación

### Diseño de la aplicación

#### Arquitectura de la aplicación

La aplicación consiste de dos módulos principales, cliente y servidor. Además se cuenta con proyectos auxiliares dentro de la solución que permiten desacoplar y reutilizar el código entre las dos aplicaciones principales.

##### Servidor

El servidor está contenido en el proyecto VaporServer. Dentro del Servidor se cuenta con una clase principal llamada Server que se encarga de manejar la interacción con el usuario administrador y de invocar a la clase ServerHandler que se encarga de manejar las requests de los clientes y mantener las conexiones con ellos.

La clase ServerHandler actua como controlador, y es la única que interactúa con la lógica, representada por la clase logic. La clase Logic es la encargada de implementar la lógica de negocio y de mantener los repositorios de los datos, que son mantenidos en memoria con excepción de las imágenes que se guardan en el directorio donde corra la aplicación.

En el proyecto VaporServer también reside la carpeta test donde se incluye la clase TestData que se encarga de insertar datos de prueba y los archivos que se usan para las carátulas de los Juegos creados por esta clase.

##### Cliente

El cliente tiene una arquitectura interna parecida a la del servidor. Se encuentra en el proyecto VaporClient. Se cuenta con una clase principal llamada Client que se encarga de la interacción con el usuario y que también instancia y hace uso de una clase ClientHandler. Esta última tiene varias funcionalidades de apoyo a la primera, por ejemplo establece la conexión, contiene métodos para ayudar a escribir y leer mensajes a través del protocolo utilizado y contiene parte de la inteligencia usada para recibir archivos.

#### Clases auxiliares

Como parte de los esfuerzos en pos de la reutilización del código, pero también para ayudar a desacoplar y entender mejor el mismo, se cuenta con una serie de proyectos/clases auxiliares donde se derivan algunas tareas repetitivas.

En el proyecto Common se tienen handlers que se encargan de manejar el NetworkStream (NetworkStreamHandler), el FileStream (FileStreamHandler) y el manejo de archivos en general (FileHandler). Como punto a destacar, el uso de estas clases siempre se hace inyectando las dependencias a través de las interfaces que se encuentran en el mismo proyecto.

El proyecto DomainObjects simplemente contiene la definición de las clases base, de manera de aportar más claridad y más orden a la codificación del proyecto. Por el momento no hay lógica de manejo de base de datos, pero de implementarse la incluiría ahi.

Por último se tiene el proyecto Protocol Library que se encarga de definir el protocolo y algunas de sus funcionalidades más utlizadas. Se ahondará más sobre el protocolo en la siguiente sección. 

#### Protocolo utilizado



#### Manejo del paralelismo


#### Manejo de la mutua exclusión



### Limitaciones y errores conocidos

Se sabe que tanto la aplicación cliente como la aplicación servidor no manejan de la manera más agraciada los casos de uso donde se parte de la lista de juegos y se sigue con alguna acción cuando no hay ningún juego cargaco.

Un ejemplo de esto es cuando se quiere comprar un juego y todavía no hay ninguno, se solicita igualmente el número de juego.


Los datos sensibles como el password no se manejan de ninguna manera particular respecto de los datos comunes.


La aplicación no siempre se comporta bien ante cortes abruptos en la comunicación.

### Funcionalidades implementadas

Para este obligatorio se implementaron todas las funcionalidades solicitadas, agregando las que habían faltado en la entrega anterior y trabajando para mejorar las existentes.

1. Conexión y desconexión de un cliente al servidor.
2. Alta, baja y modificación de usuarios (sólo servidor).
3. Login y logout al sistema.
4. Ver catálogo de juegos disponibles.
5. Que un usuario adquiera un juego.
6. Ver los juegos que adquiridos por el usuario.
7. Publicar un juego.
8. Publicar la carátula del juego.
9. Descargar una carátula de un juego.
10. Publicar una calificación del juego (luego del promedio se va obtener la nota del juego).
11. Publicar una reseña escrita de un juego.
12. Buscar Juegos (por los diferentes filtros ya definidos).

### Información para el test

Al compilar la aplicación se crean los ejecutables para el cliente y el servidor. Están en \VaporClient\bin\Debug\net5.0 y \VaporServer\bin\Debug\net5.0 respectivamente.

Se provee la funcionalidad de carga de datos de prueba en la aplicación servidor. Para ejecutarla solamente tienen que seleccionar la opción 99 en el menú. A continuación se provee un resumen de los datos de prueba:

Usuarios:

| UserName | Password |
|----------|----------|
| Jorge    | Jorge    |
| Maria    | Maria    |

Juegos:

| Nombre                     | Género    | Carátula             |
|----------------------------|-----------|----------------------|
| Paper Mario                | Arcade    | PaperMario.jpeg      |
| Microsoft Flight Simulator | Simulador | FlightSimulator.jpeg |
| Wasteland 3                | RPG       | Wasteland3.jpeg      |

Juegos de Jorge:

| Nombre                     |
|----------------------------|
| Paper Mario                |
| Microsoft Flight Simulator |

Juegos de María:

| Nombre                     |
|----------------------------|
| Microsoft Flight Simulator |
| Wasteland 3                |

Reviews:

| Usuario | Juego                      | Nota | Titulo                                |
|---------|----------------------------|------|---------------------------------------|
| Jorge   | Paper Mario                | 3    | Ta bueno                              |
| Jorge   | Microsoft Flight Simulator | 5    | Me encanta este juego                 |
| Maria   | Microsoft Flight Simulator | 4    | Esta demas, falta un poco de graficos |
| Maria   | Wasteland 3                | 1    | No me gustó, un bajon                 |

