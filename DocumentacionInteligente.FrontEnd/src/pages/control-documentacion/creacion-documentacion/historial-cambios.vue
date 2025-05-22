<template>
  <table class="history-changes tw-w-full tw-mb-6">
    <thead>
      <tr>
        <td>NÚMERO DE REVISIÓN</td>
        <td>FECHA</td>
        <td>DESCRIPCIÓN DEL CAMBIO</td>
      </tr>
    </thead>
    <tbody>
      <tr v-for="(item, index) in content" :key="index">
        <td>{{ item.number }}</td>
        <td>{{ item.date }}</td>
        <td>{{ item.description }}</td>
      </tr>
    </tbody>
  </table>
</template>

<script setup>
defineProps({
  content: {
    type: Array,
    required: true
  }
})
</script>


<style scoped lang="scss">
.history-changes {
  width: 100%;
  tr, td {
    border: 1px solid black;
  }

  thead {
    tr {
      td {
        font-weight: bold;
        text-align: center;
      }

      td:first-child {
        width: 30%;
      }

      td:nth-child(2) {
        width: 20%;
      }

      td:nth-child(3) {
        width: 55%;
      }
    }
  }
}
</style>
