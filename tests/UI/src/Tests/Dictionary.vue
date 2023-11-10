<script setup>
import { reactive, ref } from 'vue'
import { testInstance } from './Dictionary';
import {ArcTable, CompositeHeader, CompositeCell} from './../../ARCtrl/index.js'

const table = ArcTable.init("MyTable")
table.AddColumn(CompositeHeader.OfHeaderString("Source"),Array.from({ length: 10 }, (_, index) => CompositeCell.createFreeText(index.toString())))

console.log(table)

const TestInstance = reactive(testInstance)

const TableInstance = reactive(table)

function removeFirst() {
    TestInstance.RemoveFirst()
    console.log("Hit Remove Dict", TestInstance)
}
function removeFirstArr() {
    TestInstance.RemoveFirstArr()
    console.log("Hit Remove Arr", TestInstance)
}

function removeTableRow() {
    TableInstance.RemoveRow(0)
    console.log("Hit Remove Table", TestInstance)
}
</script>

<template>
    <section>
        <header>Dictionary</header>
        <section>
            <button id="ReactivityButton" @click="removeFirst">Dictionary: {{ TestInstance.CellCount }}</button>
            <button id="ReactivityButtonArr" @click="removeFirstArr">ResizeArray: {{ TestInstance.CellCountArr }}</button>
        </section>
        <section>
            <button id="ArcTableReactivityButton" @click="removeTableRow">ArcTable Rows: {{ TableInstance.RowCount }}</button>
        </section>
    </section>
</template>

<style scoped>
section {
    margin: 1rem
}
header {
    font-weight: bold;
    font-size: large;
    margin-bottom: .5rem;
}
</style>
