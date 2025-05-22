<template>
  <table class="document-header">
    <tbody>
      <tr>
        <td rowspan="3">
          <img src="/img/SistemaIntegrado/header_logo.jpg" width="100%" class="img-header" />
        </td>
        <td rowspan="2" colspan="2">{{ title }}</td>
        <td>Página {{ sheet }} de {{ total_sheets }}</td>
      </tr>
      <tr>
        <td>Código: {{ code }}</td>
      </tr>
      <tr>
        <td>Fecha Edición: {{ edit_date }}</td>
        <td>Versión: {{ version }}</td>
        <td>Fecha de Divulgación: </td>
      </tr>
      <tr>
        <td>Elaboró: {{ person_prepared }}</td>
        <td colspan="2">Revisó: {{ person_reviewed }}</td>
        <td>Autorizó: {{ person_authorized }}</td>
      </tr>
    </tbody>
  </table>
</template>

<script setup>
defineProps({
  title: String,
  edit_date: String,
  sheet: Number,
  total_sheets: Number,
  code: String,
  version: String,
  person_prepared: String,
  person_authorized: String,
  person_reviewed: String
})
</script>


<style scoped lang="scss">
.document-header {
  width: 100%;

  table {
    border-spacing: 0;
    width: 100%;
  }

  td {
    border: 1px solid black;
  }

  .img-header {
    opacity: 0.5;
  }
}

</style>
