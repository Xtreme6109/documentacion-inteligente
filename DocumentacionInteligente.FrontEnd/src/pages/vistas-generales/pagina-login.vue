<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const correo = ref('')
const password = ref('')
const router = useRouter()
const error = ref('')

const baseURL = 'http://localhost:5168/api/User/login'

const login = async () => {
  error.value = ''
  try {
    const response = await axios.post(baseURL, {
      correo: correo.value,
      password: password.value
    })

    const token = response.data.token

    localStorage.setItem('token', token)

    router.push('/inicio')
  } catch (err) {
    if (err.response?.data?.message) {
      error.value = err.response.data.message
    } else {
      error.value = 'Error al iniciar sesión'
    }
  }
}
</script>
<template>
  <div class="background">
    <div class="shape"></div>
    <div class="shape"></div>
  </div>

  <form @submit.prevent="login">
    <h3>Iniciar Sesión</h3>

    <label for="usuario">Correo Electrónico</label>
    <input class="login-input" type="text" placeholder="Correo o Usuario" id="usuario" v-model="correo" />

    <label for="contrasegna">Contraseña</label>
    <input class="login-input" type="password" placeholder="Contraseña" id="contrasegna" v-model="password" />

    <button type="submit" class="login-button">Ingresar</button>

    <div v-if="error" class="error-message">{{ error }}</div>

    <div class="forgot-password">
      <a href="#" class="text-light">¿Has olvidado la contraseña?</a>
    </div>
  </form>
</template>

<style lang="scss">
canvas {
  display: block;
}
/* ---- tsparticles container ---- */
#tsparticles {
  position: absolute;
  width: 100%;
  height: 100%;
  background-color: #323031;
  background-image: url('');
  background-repeat: no-repeat;
  background-size: cover;
  background-position: 50% 50%;
}

*,
*:before,
*:after {
  padding: 0;
  margin: 0;
  box-sizing: border-box;
}

.background {
  width: 430px;
  height: 520px;
  position: absolute;
  transform: translate(-50%, -50%);
  left: 50%;
  top: 50%;
}

.background .shape {
  height: 200px;
  width: 200px;
  position: absolute;
  border-radius: 50%;
}

.shape:first-child {
  background: linear-gradient(#1845ad, #23a2f6);
  left: -80px;
  top: -80px;
}

.shape:last-child {
  background: linear-gradient(to right, #ff512f, #f09819);
  right: -30px;
  bottom: -80px;
}

form {
  height: 520px;
  width: 400px;
  background-color: #333; /* Fondo oscuro del formulario */
  position: absolute;
  transform: translate(-50%, -50%);
  top: 50%;
  left: 50%;
  border-radius: 10px;
  backdrop-filter: blur(10px);
  border: 2px solid #444; /* Borde oscuro */
  box-shadow: 0 0 40px rgba(8, 7, 16, 0.6);
  padding: 50px 35px;
}

form * {
  font-family: 'Poppins', sans-serif;
  color: #ffffff;
  letter-spacing: 0.5px;
  outline: none;
  border: none;
}

form h3 {
  font-size: 32px;
  color: #ffffff;
  font-weight: 500;
  line-height: 42px;
  text-align: center;
}

label {
  display: block;
  margin-top: 30px;
  font-size: 16px;
  font-weight: 500;
}

.login-input {
  display: block;
  height: 50px;
  width: 100%;
  color: #333;
  background-color: #fff;
  border-radius: 3px;
  padding: 0 10px;
  margin-top: 8px;
  font-size: 14px;
  font-weight: 300;
}

::placeholder {
  color: #ccc; /* Color más claro para los placeholder */
}

.login-button {
  all: unset;
  display: block;
  text-align: center;
  margin-top: 50px;
  width: 100%;
  background-color: #555; /* Fondo oscuro del botón */
  color: #fff; /* Color de texto claro */
  padding: 15px 0;
  font-size: 18px;
  font-weight: 600;
  border-radius: 5px;
  cursor: pointer;
}

.login-button:hover {
  background-color: #444; /* Efecto hover para el botón */
}

.social {
  margin-top: 30px;
  display: flex;
}

.social div {
  background: #444; /* Fondo oscuro para los botones sociales */
  width: 150px;
  border-radius: 3px;
  padding: 5px 10px 10px 5px;
  color: #eaf0fb;
  text-align: center;
}

.social div:hover {
  background-color: #555; /* Efecto hover para botones sociales */
}

.social .fb {
  margin-left: 25px;
}

.social i {
  margin-right: 4px;
}

.forgot-password {
  margin-top: 20px;
  text-align: center;
}

.forgot-password a {
  color: #00dbd3; /* Color de enlace */
  text-decoration: none;
  font-size: 14px;
}

.forgot-password a:hover {
  text-decoration: none;
  color: #00716d;
}
</style>
