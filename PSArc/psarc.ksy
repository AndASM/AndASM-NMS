meta:
  id: ps_arc
  file-extension: pak
  endian: be

seq:
  - id: header
    type: header_type
  - id: toc
    type: toc_type
    
types:
  header_type:
    seq:
      - id: magic
        contents: PSAR
      - id: version
        type: version
      - id: compression
        type: str
        encoding: UTF-8
        size: 4
      - id: toc_length
        type: u4
      - id: toc_entry_size
        type: u4
      - id: toc_entry_count
        type: u4
      - id: block_size
        type: u4
      - id: archive_flags
        type: header_flags
  version:
    seq:
      - id: major
        type: u2
      - id: minor
        type: u2
  header_flags:
    seq:
      - id: unused
        type: b1
        repeat: expr
        repeat-expr: 29
      - id: absolute
        type: b1
      - id: ignorecase
        type: b1
      - id: relative
        type: b1
  toc_type:
    seq:
      - id: entries
        type: toc_entry
        size: _root.header.toc_entry_size
        repeat: expr
        repeat-expr: _root.header.toc_entry_count
  toc_entry:
    seq:
      - id: md5_hash
        size: 16
      - id: start_index
        type: u4
      - id: size
        type: u5
      - id: offset
        type: u5
    instances:
      body:
        io: _root._io
        pos: offset.value
        type: 
          switch-on: _root.header.compression
          cases:
            "'zlib'": zlib_body_type
            "'lzma'": lzma_body_type
            _: raw_body_type
    types:
      zlib_body_type:
        seq:
          - id: content
            size: _parent.size.value
            process: zlib
      lzma_body_type:
        seq:
          - id: content
            size: _parent.size.value
            process: lzma
      raw_body_type:
        seq:
          - id: content
            size: _parent.size.value
  u5:
    seq:
      - id: packed
        size: 5
    instances:
      value:
        value: >-
          (packed[0] << 32) + (packed[1] << 24) + (packed[2] << 16) +
          (packed[3] << 8) + packed[4]

      
      