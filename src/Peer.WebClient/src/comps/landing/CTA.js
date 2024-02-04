import React from 'react'
import { SectionTitle } from '../../layout/SectionTitle'

const CTA = () => {
    return (
        <div className='flex flex-col items-center space-y-4 mx-auto md:w-2/3 lg:w-1/2'>
            <p className='text-xl font-bold text-gray-700 text-center w-full'>Stay in the Loop</p>
            <p className='text-gray-700 text-justify w-full'>Subscribe to our newsletter for platform development insights and early access to public beta.</p>
            <form
                onSubmit={e => e.preventDefault()}
                className='flex flex-row rounded-full border-2 border-gray-400 justify-between px-4 py-2 w-full focus-within:border-2 focus-within:border-indigo-500'>
                <input
                    type="email"
                    required={true}
                    name="title"
                    placeholder='Your email here..'
                    className="w-full bg-transparent text-lg border-transparent focus:border-transparent focus:ring-0 caret-indigo-500">
                </input>
                <button
                    className='text-lg font-semibold px-4 rounded-full text-indigo-500 hover:text-indigo-600 cursor-pointer'>
                    Subscribe
                </button>
            </form>
        </div>
    )
}

export default CTA