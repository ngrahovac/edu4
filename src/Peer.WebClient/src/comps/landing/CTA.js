import React from 'react'
import { SectionTitle } from '../../layout/SectionTitle'

const CTA = () => {
    return (
        <div className='flex items-center space-y-4 mx-auto w-1/2'>
            <form
                onSubmit={e => e.preventDefault()}
                className='flex flex-row rounded-full border-2 border-gray-400 justify-between px-4 py-2 w-full focus-within:border-2 focus-within:border-indigo-500'>
                <input
                    type="email"
                    required={true}
                    name="title"
                    placeholder='Your email here..'
                    className="bg-transparent text-lg border-transparent focus:border-transparent focus:ring-0 caret-indigo-500">
                </input>
                <button
                    className='text-lg font-semibold px-4 rounded-full text-indigo-500 hover:text-indigo-600 cursor-pointer'>
                    Stay in the loop
                </button>
            </form>
        </div>
    )
}

export default CTA