function mostrar(x) {


    var x = document.getElementsByClassName(x);
    var i
    var j = document.getElementsByClassName(x).length;
    console.log(x,j);
    for (i = 0; i < x.length; i++) {
        if (x.style.display === "none") {
            x.style.display = "block";
        } else {
            x.style.display = "none";
        }
    }
}



function mandarMail() {
	let nombre = document.getElementById("name").value;
	let mensaje = document.getElementById("message").value;
	let correo = document.getElementById("email").value;

	let formatoSubjet =
		"Un posible cliente " + nombre + " te mandó un mensajede desde la pagina!";
	let formatoBody =
		"Nombre de cliente: " +
		nombre +
		" <br> correo del cliente:  " +
		correo +
		" <br> mensaje: " +
		mensaje;
	var alerta =
		"Gracias " +
		nombre +
		" nos estaremos comunicando contigo lo mas pronto posible!";

	if (
		/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(correo) &&
		nombre &&
		mensaje
	) {
		Email.send({
			SecureToken: "c9f72051-5c2c-4ab0-8963-6e173366d151",
			To: "sergiobam159@gmail.com",
			From: "cuentapruebtest@gmail.com",
			Subject: formatoSubjet,
			Body: formatoBody,
		}).then((message) => {
			if (message == "OK") {
				alert(alerta);
			}
		});
	} else {
		alert("ingrese todos los campos correctamente!");
	}

	/*
	Email.send({
		SecureToken: "c9f72051-5c2c-4ab0-8963-6e173366d151",
		To: "sergiobam159@gmail.com",
		From: "cuentapruebtest@gmail.com",
		Subject: formatoSubjet,
		Body: formatoBody,
	}).then((message) => {
		if (message == "OK") {
			alert(alerta);
		}
	});*/
}

function mandarMailSolicitud() {
	let paquete = document.querySelector("#seleccionDepaquetes").value;
	let nombre = document.getElementById("nombre").value;

	let correo = document.getElementById("email").value;
	let celular = document.getElementById("celular").value;
	let empresa = document.getElementById("empresa").value;

	let formatoSubjet = "El cliente " + nombre + " solicitó un producto! ";

	let formatoBody =
		"Paquete Solicitado: " +
		paquete +
		"<br> Nombre de cliente: " +
		nombre +
		"<br> correo del cliente: " +
		correo +
		"<br> celular:" +
		celular +
		"<br> empresa: " +
		empresa;

	var alerta =
		"Gracias " +
		nombre +
		" nos estaremos comunicando contigo lo mas pronto posible!";

	console.log(formatoBody);

	if (
		/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(correo) &&
		paquete &&
		nombre &&
		celular &&
		empresa
	) {
		Email.send({
			SecureToken: "c9f72051-5c2c-4ab0-8963-6e173366d151",
			To: "sergiobam159@gmail.com",
			From: "cuentapruebtest@gmail.com",
			Subject: formatoSubjet,
			Body: formatoBody,
		}).then((message) => {
			if (message == "OK") {
				alert(alerta);
			}
		});
	} else {
		alert("ingrese todos los campos correctamente!");
	}
}

function getPaquete(nombrepaquete) {
	let paquete = document.querySelectorAll("#paquete");
	localStorage.setItem("paquete", nombrepaquete);
}
function setPaquete() {
	let paquete = localStorage.getItem("paquete");
	console.log(paquete);
}

function seleccionarMod() {
	let nombrePaquete = localStorage.getItem("paquete");
	console.log(nombrePaquete);

	document.querySelector("#seleccionDepaquetes").value = nombrePaquete;

	listaDinamica();
}

function listaDinamica() {
	let paquete = document.querySelector("#seleccionDepaquetes").value;

	if (paquete == "Facturación") {
		let lista = document.getElementById("listaDeModulos");
		lista.innerHTML = "";

		let modulo =
			'<li class="list-group-item"> Ventas</li>' +
			'<li class="list-group-item">Facturación Electrónica</li>' +
			'<li class= "list-group-item"> Gestión Comercial</li>' +
			'<li class="list-group-item"> Libro de Ventas</li>';

		lista.innerHTML = modulo;
	}

	if (paquete == "Básico") {
		let lista = document.getElementById("listaDeModulos");
		lista.innerHTML = "";

		let modulo =
			'<li class="list-group-item"> Módulo Contable</li>' +
			'<li class="list-group-item"> Módulo Cuentas a Pagar</li>' +
			'<li class="list-group-item"> Módulo de Tesoreria</li>' +
			'<li class="list-group-item"> Módulo de Viajes y Gastos</li>' +
			'<li class="list-group-item"> Módulo de Proveedores</li>';

		lista.innerHTML = modulo;
	}

	if (paquete == "Estándar") {
		let lista = document.getElementById("listaDeModulos");
		lista.innerHTML = "";

		let modulo =
			'<li class="list-group-item"> Módulo Contable</li>' +
			'<li class="list-group-item"> Módulo Cuentas a Pagar</li>' +
			'<li class="list-group-item"> Módulo de Tesorería</li>' +
			'<li class="list-group-item"> Módulo de Viajes y Gastos</li>' +
			'<li class="list-group-item"> Módulo de Proveedores</li>' +
			'<li class="list-group-item"> Módulo Almacenes</li>' +
			'<li class="list-group-item"> Módulo Compras</li>' +
			'<li class="list-group-item"> Módulo Artículos</li>';

		lista.innerHTML = modulo;
	}

	if (paquete == "Full") {
		let lista = document.getElementById("listaDeModulos");
		lista.innerHTML = "";

		let modulo =
			'<li class="list-group-item"> Modulo Contable</li>' +
			'<li class="list-group-item"> Módulo Cuentas a Pagar</li>' +
			'<li class="list-group-item"> Módulo de Tesorer&iacute;a</li>' +
			'<li class="list-group-item">Módulo de Viajes y Gastos</li>' +
			'<li class="list-group-item">Módulo de Proveedores</li>' +
			'<li class="list-group-item"> Módulo Almacenes</li>' +
			'<li class="list-group-item"> Módulo Compras</li>' +
			'<li class="list-group-item"> Módulo Artículos</li>' +
			'<li class="list-group-item"> Módulo Gestión de persona</li>' +
			'<li class="list-group-item"> Módulo Nóminas</li>' +
			'<li class="list-group-item"> Módulo Cuentas del personal</li>';

		lista.innerHTML = modulo;
	}
}
